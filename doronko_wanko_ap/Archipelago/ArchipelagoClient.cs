﻿using System;
using System.Linq;
using System.Threading;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Packets;
using doronko_wanko_ap.Handlers;
using static System.Collections.Specialized.BitVector32;

namespace doronko_wanko_ap.Archipelago
{
    public class ArchipelagoClient
    {
        public const string APVersion = "0.6.0";
        private const string Game = "DORONKO WANKO";

        public static bool Authenticated;
        private bool attemptingConnection;

        public static ArchipelagoData ServerData = new();
        private ArchipelagoSession session;

        public ItemHandler ItemHandler = new();
        public LocationHandler LocationHandler = new();
        public GoalHandler GoalHandler = new();

        /// <summary>
        /// call to connect to an Archipelago session. Connection info should already be set up on ServerData
        /// </summary>
        /// <returns></returns>
        public void Connect()
        {
            Plugin.BepinLogger.LogInfo("Connect, Attempting Connection");
            if (Authenticated || attemptingConnection) return;

            try
            {
                session = ArchipelagoSessionFactory.CreateSession(ServerData.Uri);
                SetupSession();
            }
            catch (Exception e)
            {
                Plugin.BepinLogger.LogError(e);
            }

            TryConnect();
        }

        /// <summary>
        /// add handlers for Archipelago events
        /// </summary>
        private void SetupSession()
        {
            session.MessageLog.OnMessageReceived += message => Plugin.BepinLogger.LogMessage(message.ToString());
            session.Items.ItemReceived += ItemHandler.OnItemReceived;
            session.Socket.ErrorReceived += OnSessionErrorReceived;
            session.Socket.SocketClosed += OnSessionSocketClosed;
        }

        /// <summary>
        /// attempt to connect to the server with our connection info
        /// </summary>
        private void TryConnect()
        {
            try
            {
                // it's safe to thread this function call but unity notoriously hates threading so do not use excessively
                ThreadPool.QueueUserWorkItem(
                    _ => HandleConnectResult(
                        session.TryConnectAndLogin(
                            Game,
                            ServerData.SlotName,
                            ItemsHandlingFlags.AllItems, // TODO make sure to change this line
                            new Version(APVersion),
                            password: ServerData.Password,
                            requestSlotData: true // ServerData.NeedSlotData
                        )));
            }
            catch (Exception e)
            {
                Plugin.BepinLogger.LogError(e);
                HandleConnectResult(new LoginFailure(e.ToString()));
                attemptingConnection = false;
            }
        }

        /// <summary>
        /// handle the connection result and do things
        /// </summary>
        /// <param name="result"></param>
        private void HandleConnectResult(LoginResult result)
        {
            string outText;
            if (result.Successful)
            {
                var success = (LoginSuccessful)result;

                ServerData.SetupSession(success.SlotData, session.RoomState.Seed);
                Authenticated = true;

                session.Locations.CompleteLocationChecksAsync(ServerData.CheckedLocations.ToArray());
                outText = $"Successfully connected to {ServerData.Uri} as {ServerData.SlotName}!";

                Plugin.BepinLogger.LogMessage(outText);
            }
            else
            {
                var failure = (LoginFailure)result;
                outText = $"Failed to connect to {ServerData.Uri} as {ServerData.SlotName}.";
                outText = failure.Errors.Aggregate(outText, (current, error) => current + $"\n    {error}");

                Plugin.BepinLogger.LogError(outText);

                Authenticated = false;
                Disconnect();
            }

            Plugin.BepinLogger.LogMessage(outText);
            attemptingConnection = false;
        }

        /// <summary>
        /// something we wrong or we need to properly disconnect from the server. cleanup and re null our session
        /// </summary>
        private void Disconnect()
        {
            Plugin.BepinLogger.LogDebug("disconnecting from server...");
            session?.Socket.DisconnectAsync();
            session = null;
            Authenticated = false;
        }

        public void SendMessage(string message)
        {
            session.Socket.SendPacketAsync(new SayPacket { Text = message });
        }

        /// <summary>
        /// something went wrong with our socket connection
        /// </summary>
        /// <param name="e">thrown exception from our socket</param>
        /// <param name="message">message received from the server</param>
        private void OnSessionErrorReceived(Exception e, string message)
        {
            Plugin.BepinLogger.LogError(e);
            Plugin.BepinLogger.LogMessage(message);
        }

        /// <summary>
        /// something went wrong closing our connection. disconnect and clean up
        /// </summary>
        /// <param name="reason"></param>
        private void OnSessionSocketClosed(string reason)
        {
            Plugin.BepinLogger.LogError($"Connection to Archipelago lost: {reason}");
            Disconnect();
        }


        /// <summary>
        /// When collecting a location in the game, send its id to the server
        /// </summary>
        public async void SendLocation(string id)
        {
            Plugin.BepinLogger.LogInfo($"Sending location: {id}");
            long apId = session.Locations.GetLocationIdFromName(Game, id);
            await session.Locations.CompleteLocationChecksAsync(apId);
        }

        public void SendGoalCompletion()
        {
            Plugin.BepinLogger.LogInfo($"Sending goal completion");
            var statusUpdatePacket = new StatusUpdatePacket();
            statusUpdatePacket.Status = ArchipelagoClientState.ClientGoal;
            session.Socket.SendPacket(statusUpdatePacket);
        }

        public string GetPlayerNameFromSlot(int slot)
        {
            return session.Players.GetPlayerName(slot) ?? "Server";
        }

        public string GetItemNameFromId(long id)
        {
            return session.Items.GetItemName(id,Game) ?? $"Item[{id}]";
        }

        public string GetLocationNameFromId(long id)
        {
            return session.Locations.GetLocationNameFromId(id,Game) ?? $"Location[{id}]";
        }

    }
}
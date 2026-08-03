using System;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

/*
 * Script Name: GameAnalyticsManager
 * Purpose: Initializes Unity Gaming Services and records player events.
 *
 * Tracked Events:
 * 1. Game started
 * 2. Lane changed
 * 3. Player jumped
 * 4. Obstacle hit
 */

public class GameAnalyticsManager : MonoBehaviour
{
    #region Singleton

    // Allows other scripts to access the analytics manager easily.
    public static GameAnalyticsManager Instance { get; private set; }

    #endregion

    #region Private Variables

    // Prevents events from being sent before Unity Services is ready.
    private bool analyticsReady;

    #endregion

    #region Unity Methods

    private async void Awake()
    {
        // Prevent duplicate analytics managers when changing scenes.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        await InitializeAnalytics();
    }

    #endregion

    #region Initialization

    private async System.Threading.Tasks.Task InitializeAnalytics()
    {
        try
        {
            // Initialize Unity Gaming Services.
            await UnityServices.InitializeAsync();

            /*
             * This starts Analytics data collection.
             * For a released game, add a proper privacy and consent screen.
             */
            AnalyticsService.Instance.StartDataCollection();

            analyticsReady = true;

            Debug.Log("Unity Analytics initialized successfully.");
        }
        catch (Exception exception)
        {
            analyticsReady = false;

            Debug.LogError(
                "Unity Analytics initialization failed: " +
                exception.Message
            );
        }
    }

    #endregion

    #region Analytics Events

    public void TrackGameStarted()
    {
        RecordEvent("game_started");
    }

    public void TrackLaneChanged()
    {
        RecordEvent("lane_changed");
    }

    public void TrackPlayerJumped()
    {
        RecordEvent("player_jumped");
    }

    public void TrackObstacleHit()
    {
        RecordEvent("obstacle_hit");
    }

    #endregion

    #region Helper Methods

    private void RecordEvent(string eventName)
    {
        // Do not attempt to send an event before Analytics is ready.
        if (!analyticsReady)
        {
            Debug.LogWarning(
                "Analytics is not ready. Event skipped: " + eventName
            );

            return;
        }

        AnalyticsService.Instance.RecordEvent(eventName);

        // This message helps verify the event during testing.
        Debug.Log("Analytics event recorded: " + eventName);
    }

    #endregion
}
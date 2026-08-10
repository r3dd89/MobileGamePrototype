using System;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UnityConsent;

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

    public static GameAnalyticsManager Instance { get; private set; }

    #endregion

    #region Private Variables

    private bool analyticsReady;

    #endregion

    #region Unity Methods

    private async void Awake()
    {
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
             * Grant Analytics consent for this course prototype.
             *
             * For a public/commercial release, this should be connected
             * to an actual user-facing privacy and consent choice.
             */
            EndUserConsent.SetConsentState(
                new ConsentState
                {
                    AnalyticsIntent = ConsentStatus.Granted,
                    AdsIntent = ConsentStatus.Denied
                }
            );

            analyticsReady = true;
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
        if (!analyticsReady)
        {
            return;
        }

        AnalyticsService.Instance.RecordEvent(eventName);
    }

    #endregion
}
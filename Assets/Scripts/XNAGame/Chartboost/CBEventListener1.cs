using UnityEngine;
using System;
using System.Collections;
using Chartboost;
using GameStateManagement;
using GameStateManagementSample;


public class CBEventListener1 : MonoBehaviour
{
    public static ScreenManager screenManager;
    public static int levelToLoad;

#if UNITY_ANDROID || UNITY_IPHONE

	void OnEnable()
	{
		// Listen to all impression-related events
		CBManager.didFailToLoadInterstitialEvent += didFailToLoadInterstitialEvent;
		CBManager.didDismissInterstitialEvent += didDismissInterstitialEvent;
		CBManager.didCloseInterstitialEvent += didCloseInterstitialEvent;
		CBManager.didClickInterstitialEvent += didClickInterstitialEvent;
		CBManager.didCacheInterstitialEvent += didCacheInterstitialEvent;
		CBManager.didShowInterstitialEvent += didShowInterstitialEvent;
		CBManager.didFailToLoadMoreAppsEvent += didFailToLoadMoreAppsEvent;
		CBManager.didDismissMoreAppsEvent += didDismissMoreAppsEvent;
		CBManager.didCloseMoreAppsEvent += didCloseMoreAppsEvent;
		CBManager.didClickMoreAppsEvent += didClickMoreAppsEvent;
		CBManager.didCacheMoreAppsEvent += didCacheMoreAppsEvent;
		CBManager.didShowMoreAppsEvent += didShowMoreAppsEvent;

        Debug.Log("CBEventListener1 onEnable");
	}


	void OnDisable()
	{
		// Remove event handlers
		CBManager.didFailToLoadInterstitialEvent -= didFailToLoadInterstitialEvent;
		CBManager.didDismissInterstitialEvent -= didDismissInterstitialEvent;
		CBManager.didCloseInterstitialEvent -= didCloseInterstitialEvent;
		CBManager.didClickInterstitialEvent -= didClickInterstitialEvent;
		CBManager.didCacheInterstitialEvent -= didCacheInterstitialEvent;
		CBManager.didShowInterstitialEvent -= didShowInterstitialEvent;
		CBManager.didFailToLoadMoreAppsEvent -= didFailToLoadMoreAppsEvent;
		CBManager.didDismissMoreAppsEvent -= didDismissMoreAppsEvent;
		CBManager.didCloseMoreAppsEvent -= didCloseMoreAppsEvent;
		CBManager.didClickMoreAppsEvent -= didClickMoreAppsEvent;
		CBManager.didCacheMoreAppsEvent -= didCacheMoreAppsEvent;
		CBManager.didShowMoreAppsEvent -= didShowMoreAppsEvent;
	}



	void didFailToLoadInterstitialEvent( string location )
	{
		Debug.Log( "didFailToLoadInterstitialEvent: " + location );
        GameplayScreen.LoadAndPlayLevel(levelToLoad, screenManager);
	}
	
	void didDismissInterstitialEvent( string location )
	{
		Debug.Log( "didDismissInterstitialEvent: " + location );
        GameplayScreen.LoadAndPlayLevel(levelToLoad, screenManager);
	}
	
	void didCloseInterstitialEvent( string location )
	{
		Debug.Log( "didCloseInterstitialEvent: " + location );
        // didDismissInterstitialEvent seems is fired on close or click events
        //GameplayScreen.LoadAndPlayLevel(levelToLoad, screenManager);
	}
	
	void didClickInterstitialEvent( string location )
	{
		Debug.Log( "didClickInterstitialEvent: " + location );
	}
	
	void didCacheInterstitialEvent( string location )
	{
		Debug.Log( "didCacheInterstitialEvent: " + location );
	}
	
	void didShowInterstitialEvent( string location )
	{
		Debug.Log( "didShowInterstitialEvent: " + location );
	}
	
	void didFailToLoadMoreAppsEvent()
	{
		Debug.Log( "didFailToLoadMoreAppsEvent" );
	}
	
	void didDismissMoreAppsEvent()
	{
		Debug.Log( "didDismissMoreAppsEvent" );
	}
	
	void didCloseMoreAppsEvent()
	{
		Debug.Log( "didCloseMoreAppsEvent" );
	}
	
	void didClickMoreAppsEvent()
	{
		Debug.Log( "didClickMoreAppsEvent" );
	}
	
	void didCacheMoreAppsEvent()
	{
		Debug.Log( "didCacheMoreAppsEvent" );
	}
	
	void didShowMoreAppsEvent()
	{
		Debug.Log( "didShowMoreAppsEvent" );
	}
			
#endif
}



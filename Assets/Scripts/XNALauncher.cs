using UnityEngine;
using System.Collections;
using XNAGame;
using FarseerPhysics;
using System;
using Assets.Scripts.XNAGame.Helpers;
using Assets.Scripts.XNAGame;

public class XNALauncher : MonoBehaviour {
    Game1 game;
	DrawQueue drawQueue;

    bool IsCrashOccured;

    VirtualViewport virtualViewport;
    Texture2D blackTexture;

    Texture2D CreateLetterBoxOrPillarBoxTexture()
    {
        UnityEngine.Texture2D blackTexture = new UnityEngine.Texture2D(1, 1, TextureFormat.ARGB32, false);
        blackTexture.SetPixel(0, 0, Color.black);
        blackTexture.Apply();
        return blackTexture;
    }

	// Use this for initialization
	void Start () 
    {
        // Close game if no internet connection
        if (!PushBlock.Helpers.Helper.CheckForInternetConnection())
        {
            Application.Quit();
        }

        // Site lock for web game
#if UNITY_WEBPLAYER11
        //UnityEngine.Application.ExternalEval(
        //    "if(document.domain != 'suslikgames.com' || window != window.top) { document.location='http://suslikgames.com'; }"
        //);
#endif

        // is used by letterbox or pillarbox
        blackTexture = CreateLetterBoxOrPillarBoxTexture();

        // This option is overridden in unity ide settings to frequency framerate (60hz)
        // Is used as fallback but maybe it is not required on most devices
        Application.targetFrameRate = 30;
        
        // Add an audio source and tell the media player to use it for playing sounds
        Microsoft.Xna.Framework.Media.MediaPlayer.AudioSource = gameObject.AddComponent<AudioSource>();

		drawQueue = new DrawQueue();
        game = new Game1();
		game.DrawQueue = drawQueue;

        // DEBUG
        // UnityEngine.Screen.SetResolution(800, 480, false);

        virtualViewport = new VirtualViewport(800, 480);
        virtualViewport.Resize();

        game.Begin();

      
	}

	// Update is called once per frame
	void Update () 
    {
        float deltaTime = Time.deltaTime;

        try
        {
            game.Tick(deltaTime);
            //DebugHelper.Current.Update();

#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                if (Input.GetKeyUp(KeyCode.Escape))
                {
                    if (Chartboost.CBBinding.onBackPressed())
                    {
                        Debug.Log("Chartboost back button processed");
                        return;
                    }
                    else
                    {
                        // What to do what to do ?
                       // Application.Quit();
                    }
                }
            }
#endif

        }
        catch (Exception e)
        {
            if (!IsCrashOccured)
            {
                IsCrashOccured = true;
                //DebugHelper.Current.SendCrashReportEmail(e);
                //DebugHelper.Current.LogError(e);
                Debug.Log(e.ToString());
                Application.Quit();
            }
        }

        drawQueue.Clear();
	}
	
    void OnEnable()
	{
		// Initialize the Chartboost plugin
#if UNITY_ANDROID
		// Remember to set the Android app ID and signature in the file `/Plugins/Android/res/values/strings.xml`
		Chartboost.CBBinding.init();
#endif

        Debug.Log("XnaLauncher onEnable");
    }

    void DrawSprites()
    {
        // Draw sprites from SpriteBatch.Draw()
        for (int i = 0; i < drawQueue.LastSpriteQueue.Length; i++)
        {
            DrawSpriteCall call = drawQueue.LastSpriteQueue[i];
            float x = call.Position.X;
            float y = call.Position.Y;
            var scaledOrigin = call.Origin * call.Scale;
            x -= scaledOrigin.X;
            y -= scaledOrigin.Y;
            float width = call.Texture2D.UnityTexture.width * call.Scale;
            float height = call.Texture2D.UnityTexture.height * call.Scale;
            GUI.color = new Color(call.Color.X, call.Color.Y, call.Color.Z, call.Color.W);

            Rect sourceRect = new Rect(0, 0, 1, 1);

            if (call.Source != null)
            {
                sourceRect.x = call.Source.Value.X / width;
                sourceRect.y = call.Source.Value.Y / height;
                sourceRect.width = call.Source.Value.Width / width;
                sourceRect.height = call.Source.Value.Height / height;
            }

            if (call.SpriteEffects == Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally)
            {
                sourceRect.x = 1 - sourceRect.x;
                sourceRect.width *= -1;
            }
            else if (call.SpriteEffects == Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipVertically)
            {
                sourceRect.y = 1 - sourceRect.y;
                sourceRect.height *= -1;
            }

            Matrix4x4 matrixBackup = GUI.matrix;

            float degrees = (float)(call.Rotation * 180 / Math.PI);
            GUIUtility.RotateAroundPivot(degrees, new Vector2(x + scaledOrigin.X, y + scaledOrigin.Y));
            GUI.DrawTextureWithTexCoords(new Rect(x, y, width * Mathf.Abs(sourceRect.width), height * Mathf.Abs(sourceRect.height)), call.Texture2D.UnityTexture, sourceRect);
            GUI.matrix = matrixBackup;
        }
    }

    void DrawStrings()
    {
        // Draw strings from SpriteBatch.DrawString()
        for (int i = 0; i < drawQueue.LastStringQueue.Length; i++)
        {
            DrawStringCall call = drawQueue.LastStringQueue[i];

            GUI.color = new Color(call.Color.X, call.Color.Y, call.Color.Z, call.Color.W);

            // Calculate full text size
            var fontSize = call.Font.MeasureString(call.Value) * call.Scale;

            float left = call.Position.X;
            float top = call.Position.Y - (fontSize.Y / 2);

            if (call.IsCentered)
            {
                left -= fontSize.X / 2;
            }

            var charHeight = fontSize.Y;

            foreach (var ch in call.Value)
            {
                var charWidth = call.Font.GetCharWidth(ch) * call.Scale;
                var charPosition = new Rect(left, top, charWidth, charHeight);
                var texCoords = call.Font.GetCharTextureCoords(ch);
                GUI.DrawTextureWithTexCoords(charPosition, call.Font.Texture.UnityTexture, texCoords);

                left += charWidth;
            }
        }
    }

    void OnGUI()
    {
        if (Event.current.type == EventType.Repaint)
        {
#if UNITY_ANDROID || UNITY_EDITOR
            virtualViewport.AdjustDrawCallsForViewport(drawQueue);
#endif
            DrawSprites();
            DrawStrings();

#if UNITY_ANDROID || UNITY_EDITOR
            DrawLetterBoxOrPillarBox();
#endif
            //DebugHelper.Current.Draw();
        }
    }

    public void DrawLetterBoxOrPillarBox()
    {
        var viewport = VirtualViewport.Current;
        Rect sourceRect = new Rect(0, 0, 1, 1);
        GUI.color = Color.black;

        // draw pillarbox
        if (viewport.ScreenAspectRatio > 1.68)
        {
            Rect leftRect = new Rect(0, 0, viewport.X, viewport.Height);
            float leftX = viewport.ScreenWidth - (viewport.X + 1);
            float rightX = viewport.X + 10;
            Rect rightRect = new Rect(leftX, 0, rightX, viewport.Height);
            GUI.DrawTextureWithTexCoords(leftRect, blackTexture, sourceRect);
            GUI.DrawTextureWithTexCoords(rightRect, blackTexture, sourceRect);
        }
        else if (viewport.ScreenAspectRatio < 1.65) // draw letterbox
        {
            Rect topRect = new Rect(0, 0, viewport.Width, viewport.Y);
            Rect bottomRect = new Rect(0, viewport.Y + viewport.Height, viewport.Width, viewport.Y);
            GUI.DrawTextureWithTexCoords(topRect, blackTexture, sourceRect);
            GUI.DrawTextureWithTexCoords(bottomRect, blackTexture, sourceRect);
        }
    }

    ///
    void OnApplicationPause(bool pauseStatus)
    {
        Debug.Log("OnApplicationPause method is called");

        if (game != null)
        {
            if (pauseStatus)
            {
                game.OnGamePaused();
                Debug.Log("Game paused");
            }
            else
            {
                // Check internet connection
                if (PushBlock.Helpers.Helper.CheckForInternetConnection())
                {
                    game.OnGameResumed();
                    Debug.Log("Game resumed");
                }
                else
                {
                    Debug.Log("No internet connection !");
                    Application.Quit();
                }
                
            }
        }
        
    }

    void OnApplicationQuit()
    {
       // DebugHelper.Current.Dispose();
    }

    public void ExecuteAsync(Action action)
    {
        StartCoroutine(_WaitAndExecute(0, action));
    }

    public void WaitAndExecute(float waitTime, Action action)
    {
        StartCoroutine(_WaitAndExecute(waitTime, action));
    }
		
    IEnumerator _WaitAndExecute(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action.Invoke();
    }


}

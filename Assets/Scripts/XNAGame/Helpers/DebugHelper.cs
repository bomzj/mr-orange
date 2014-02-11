using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PushBlock.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using XNAGame;

namespace Assets.Scripts.XNAGame.Helpers
{
    class DebugHelper : IDisposable
    {
        private const string LogFileName = "log.txt";
        private const string LogDateTimeFormat = "dd.MM.yyyy HH:mm:ss";

        private string LogFileFullPath { get; set; }
        private StreamWriter logWriter { get; set; }

        private GUIStyle style;
        private string errorLog;

        // FPS
        public float updateInterval = 0.5F;
        private float accum = 0; // FPS accumulated over the interval
        private int frames = 0; // Frames drawn over the interval
        private float timeleft; // Left time for current interval
        float fps;

        private static DebugHelper current = null;
        
        public static DebugHelper Current 
        {
            get
            {
                if (current == null)
                {
                    current = new DebugHelper();
                }
                return current;
            }
        }


        public DebugHelper()
        {
            style = new GUIStyle();
            timeleft = updateInterval;

            // full path to log file
            LogFileFullPath = Application.persistentDataPath + "/" + LogFileName;
#if !UNITY_WEBPLAYER
            if (File.Exists(LogFileFullPath))
            {
                File.Delete(LogFileFullPath);
            }
            

            logWriter = File.CreateText(LogFileFullPath);
            logWriter.AutoFlush = true;
#endif
        }

        public void UpdateFPS()
        {
            timeleft -= Time.deltaTime;
            accum += Time.timeScale / Time.deltaTime;
            ++frames;

            // Interval ended - update GUI text and start new interval
            if (timeleft <= 0.0)
            {
                // display two fractional digits (f2 format)
                fps = accum / frames;

                timeleft = updateInterval;
                accum = 0.0F;
                frames = 0;
            }
        }

        public void SendCrashReportEmail(Exception e)
        {
            string emailFrom = "shamihulau@gmail.com";
            string emailTo = "diavollo@tut.by";
            string body = e.ToString();
            string subject = "Crash report";
            Helper.SendEmail(emailFrom, emailTo, subject, body);
        }

        public void DrawFPS()
        {
            style.fontSize = 15;
            VirtualViewport viewport = VirtualViewport.Current;
            GUI.Label(new Rect(viewport.X, viewport.Y, 200, 100), "Fps: " + (int)fps, style);
        }

        public void DrawLog()
        {
            style.fontSize = 12;
            GUI.Label(new Rect(0, 30, 200, 300), errorLog, style);
        }

        public void LogError(Exception e)
        {
            Log(e.ToString());
        }

        public void Log(string message)
        {
            var date = DateTime.Now.ToString(LogDateTimeFormat);
            logWriter.WriteLine("[{0}]   {1}", date, message);
        }

        public void Update()
        {
            UpdateFPS();
        }

        public void Draw()
        {
            //GUI.color = UnityEngine.Color.yellow;
            //GUI.contentColor = UnityEngine.Color.yellow;
            //DrawFPS();
            //DrawLog();
        }

        public void Dispose()
        {
            logWriter.Dispose();
        }
    }
}

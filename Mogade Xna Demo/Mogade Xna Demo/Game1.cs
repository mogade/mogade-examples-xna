using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Mogade.WindowsPhone;
using Mogade;

namespace Mogade_Xna_Demo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D boxTexture;

        SpriteFont Font14;
        SpriteFont Font18;
        SpriteFont Font24;

        int level;
        int score;
        int totalscores;

        string scoreboardRanks = "";

        TouchCollection touchLocations;

        List<Box> boxlist = new List<Box>();
        List<ScoreboardEntry> sblist = new List<ScoreboardEntry>();

        public enum ActiveScreen
        {
            MainMenu,
            InGame,
            GameOver,
            HighScores
        }

        public enum ActiveHighScoresTab
        {
            Overall,
            Weekly,
            Daily
        }

        ActiveScreen currentscreen = ActiveScreen.MainMenu;
        ActiveHighScoresTab currenthighscorestab = ActiveHighScoresTab.Overall;

        // Moage Stuff Start

        public IMogadeClient Mogade { get; private set; }

        //private static readonly LeaderboardScope[] _scopeOrder = new[] { LeaderboardScope.Overall, LeaderboardScope.Weekly, LeaderboardScope.Daily };
        //private LeaderboardScope _scope;
        //private int _page;
        //int userRank;
        //int userScore;
        //int page = 1;

        // Mogade Stuff End

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.SupportedOrientations = DisplayOrientation.Portrait;
            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferredBackBufferWidth = 480;
            graphics.IsFullScreen = true;

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);

            Mogade = MogadeHelper.CreateInstance();
            Mogade.LogApplicationStart();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            boxTexture = Content.Load<Texture2D>("Bitmap1");

            Font14 = Content.Load<SpriteFont>("Font14");
            Font18 = Content.Load<SpriteFont>("Font18");
            Font24 = Content.Load<SpriteFont>("Font24");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (currentscreen == ActiveScreen.MainMenu)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();

                touchLocations = TouchPanel.GetState();
                if (touchLocations.Count > 0 && touchLocations[0].State == TouchLocationState.Pressed)
                {
                    Vector2 touchposition = touchLocations[0].Position;

                    if (touchposition.X > 140 && touchposition.X < 340 && touchposition.Y > 280 && touchposition.Y < 320)
                    {
                        currentscreen = ActiveScreen.InGame;
                        resetdata();
                        setuplevel();
                    }
                    else if (touchposition.X > 140 && touchposition.X < 340 && touchposition.Y > 350 && touchposition.Y < 390)
                    {
                        LoadLeaderboard(LeaderboardScope.Overall, 1);
                        currentscreen = ActiveScreen.HighScores;
                    }
                }
            }
            else if (currentscreen == ActiveScreen.InGame)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    currentscreen = ActiveScreen.MainMenu;

                touchLocations = TouchPanel.GetState();

                for (int i = 0; i < boxlist.Count; i++)
                {
                    boxlist[i].update(elapsedTime);

                    if (boxlist[i].clicktimer <= 0)
                    {
                        currentscreen = ActiveScreen.GameOver;
                        if (Guide.IsVisible == false)
                        {
                            object stateObj;
                            Guide.BeginShowKeyboardInput(PlayerIndex.One, "Game Over", "Please enter a name for the scoreboard (No longer than 30 characters)", "", GetText, stateObj = (object)"GetText for Input PlayerOne");
                        }
                    }

                    if (touchLocations.Count > 0 && touchLocations[0].State == TouchLocationState.Pressed)
                    {
                        Vector2 touchposition = touchLocations[0].Position;

                        if (boxlist[i].rectangle.Contains(new Point((int)touchposition.X, (int)touchposition.Y)) && boxlist[i].isgreen)
                        {
                            score += 25;
                            boxlist.Remove(boxlist[i]);
                            break;
                        }
                    }
                }

                if (boxlist.Count <= 0)
                {
                    level++;
                    if (level <= 6)
                    {
                        setuplevel();
                    }
                    else
                    {
                        currentscreen = ActiveScreen.GameOver;
                        if (Guide.IsVisible == false)
                        {
                            object stateObj;
                            Guide.BeginShowKeyboardInput(PlayerIndex.One, "Game Over", "Please enter a name for the scoreboard (No longer than 10 characters)", "", GetText, stateObj = (object)"GetText for Input PlayerOne");
                        }
                    }
                }
            }
            else if (currentscreen == ActiveScreen.GameOver)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    currentscreen = ActiveScreen.MainMenu;
            }
            else if (currentscreen == ActiveScreen.HighScores)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    currentscreen = ActiveScreen.MainMenu;

                 touchLocations = TouchPanel.GetState();
                 if (touchLocations.Count > 0 && touchLocations[0].State == TouchLocationState.Pressed)
                 {
                     Vector2 touchposition = touchLocations[0].Position;

                     if (touchposition.X > 100 && touchposition.X < 140 && touchposition.Y > 75 && touchposition.Y < 130)
                     {
                         if (currenthighscorestab == ActiveHighScoresTab.Overall)
                         {
                             currenthighscorestab = ActiveHighScoresTab.Daily;
                         }
                         else
                         {
                             currenthighscorestab--;
                         }

                         if (currenthighscorestab == ActiveHighScoresTab.Overall)
                         {
                             LoadLeaderboard(LeaderboardScope.Overall, 1);
                         }
                         else if (currenthighscorestab == ActiveHighScoresTab.Weekly)
                         {
                             LoadLeaderboard(LeaderboardScope.Weekly, 1);
                         }
                         else if (currenthighscorestab == ActiveHighScoresTab.Daily)
                         {
                             LoadLeaderboard(LeaderboardScope.Daily, 1);
                         }
                     }
                     else if (touchposition.X > 340 && touchposition.X < 380 && touchposition.Y > 75 && touchposition.Y < 130)
                     {
                         if (currenthighscorestab == ActiveHighScoresTab.Daily)
                         {
                             currenthighscorestab = ActiveHighScoresTab.Overall;
                         }
                         else
                         {
                             currenthighscorestab++;
                         }

                         if (currenthighscorestab == ActiveHighScoresTab.Overall)
                         {
                             LoadLeaderboard(LeaderboardScope.Overall, 1);
                         }
                         else if (currenthighscorestab == ActiveHighScoresTab.Weekly)
                         {
                             LoadLeaderboard(LeaderboardScope.Weekly, 1);
                         }
                         else if (currenthighscorestab == ActiveHighScoresTab.Daily)
                         {
                             LoadLeaderboard(LeaderboardScope.Daily, 1);
                         }
                     }
                 }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            spriteBatch.Begin();

            spriteBatch.DrawString(Font18, "Mogade.com", new Vector2(10, 10), Color.White);

            if (currentscreen == ActiveScreen.MainMenu)
            {
                spriteBatch.DrawString(Font24, "Main Menu", new Vector2(240 - (Font24.MeasureString("Main Menu").X / 2), 100), Color.White);
                spriteBatch.DrawString(Font18, "Play Game", new Vector2(240 - (Font18.MeasureString("Play Game").X / 2), 300), Color.White);
                spriteBatch.DrawString(Font18, "High Scores", new Vector2(240 - (Font18.MeasureString("High Scores").X / 2), 370), Color.White);
            }
            else if (currentscreen == ActiveScreen.InGame)
            {
                spriteBatch.DrawString(Font18, "Level: " + level.ToString(), new Vector2(50, 100), Color.White);
                spriteBatch.DrawString(Font18, "Score: " + score.ToString(), new Vector2(300, 100), Color.White);
                spriteBatch.DrawString(Font14, "Tap the squares when they turn green", new Vector2(35, 650), Color.White);

                for (int i = 0; i < boxlist.Count; i++)
                {
                    spriteBatch.Draw(boxTexture, boxlist[i].rectangle, boxlist[i].colour);
                }
            }
            else if (currentscreen == ActiveScreen.GameOver)
            {
                spriteBatch.DrawString(Font24, "Game Over", new Vector2(240 - (Font24.MeasureString("Game Over").X / 2), 100), Color.White);
                spriteBatch.DrawString(Font18, "Level: " + level.ToString(), new Vector2(100, 220), Color.White);
                spriteBatch.DrawString(Font18, "Score: " + score.ToString(), new Vector2(100, 250), Color.White);
                spriteBatch.DrawString(Font18, scoreboardRanks, new Vector2(100, 300), Color.White);
                spriteBatch.DrawString(Font18, "Rivals", new Vector2(100, 440), Color.White);
                if (sblist.Count > 0)
                {
                    for (int i = 0; i < sblist.Count; i++)
                    {
                        spriteBatch.DrawString(Font18, sblist[i].username, new Vector2(40, 480 + i * 50), Color.White);
                        spriteBatch.DrawString(Font18, "Level " + sblist[i].level, new Vector2(190, 480 + i * 50), Color.White);
                        spriteBatch.DrawString(Font18, sblist[i].points.ToString(), new Vector2(340, 480 + i * 50), Color.White);
                    }
                }
                spriteBatch.DrawString(Font18, "Total Scores: " + totalscores, new Vector2(100, 700), Color.White);
            }
            else if (currentscreen == ActiveScreen.HighScores)
            {
                switch (currenthighscorestab)
                {
                    case ActiveHighScoresTab.Overall:
                        spriteBatch.DrawString(Font18, "Overall", new Vector2(240 - (Font18.MeasureString("Overall").X / 2), 100), Color.White);
                        break;
                    case ActiveHighScoresTab.Weekly:
                        spriteBatch.DrawString(Font18, "Weekly", new Vector2(240 - (Font18.MeasureString("Weekly").X / 2), 100), Color.White);
                        break;
                    case ActiveHighScoresTab.Daily:
                        spriteBatch.DrawString(Font18, "Daily", new Vector2(240 - (Font18.MeasureString("Daily").X / 2), 100), Color.White);
                        break;
                }

                spriteBatch.DrawString(Font24, "<", new Vector2(120, 95), Color.White);
                spriteBatch.DrawString(Font24, ">", new Vector2(360 - Font24.MeasureString(">").X, 95), Color.White);

                if (sblist.Count > 0)
                {
                    for (int i = 0; i < sblist.Count; i++)
                    {
                        spriteBatch.DrawString(Font14, sblist[i].username, new Vector2(40, 220 + i * 50), Color.White);
                        spriteBatch.DrawString(Font14, "Level " + sblist[i].level, new Vector2(200, 220 + i * 50), Color.White);
                        spriteBatch.DrawString(Font14, sblist[i].points.ToString(), new Vector2(370, 220 + i * 50), Color.White);
                    }
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void resetdata()
        {
            level = 1;
            score = 0;
        }

        public void setuplevel()
        {
            boxlist.Clear();

            if (level == 1)
            {
                boxlist.Add(new Box(200, 350, 80, 80, Color.White, 2.0f));
            }
            else if (level == 2)
            {
                boxlist.Add(new Box(150, 350, 80, 80, Color.White, 1.5f));
                boxlist.Add(new Box(250, 350, 80, 80, Color.White, 2.0f));
            }
            else if (level == 3)
            {
                boxlist.Add(new Box(100, 350, 80, 80, Color.White, 1.5f));
                boxlist.Add(new Box(200, 350, 80, 80, Color.White, 2.0f));
                boxlist.Add(new Box(300, 350, 80, 80, Color.White, 2.5f));
            }
            else if (level == 4)
            {
                boxlist.Add(new Box(200, 300, 80, 80, Color.White, 1.5f));
                boxlist.Add(new Box(100, 400, 80, 80, Color.White, 2.0f));
                boxlist.Add(new Box(200, 400, 80, 80, Color.White, 2.5f));
                boxlist.Add(new Box(300, 400, 80, 80, Color.White, 1.0f));
            }
            else if (level == 5)
            {
                boxlist.Add(new Box(150, 300, 80, 80, Color.White, 1.5f));
                boxlist.Add(new Box(250, 300, 80, 80, Color.White, 2.0f));
                boxlist.Add(new Box(100, 400, 80, 80, Color.White, 2.5f));
                boxlist.Add(new Box(200, 400, 80, 80, Color.White, 2.5f));
                boxlist.Add(new Box(300, 400, 80, 80, Color.White, 1.5f));
            }
            else if (level == 6)
            {
                boxlist.Add(new Box(100, 300, 80, 80, Color.White, 1.5f));
                boxlist.Add(new Box(200, 300, 80, 80, Color.White, 2.0f));
                boxlist.Add(new Box(300, 300, 80, 80, Color.White, 2.5f));
                boxlist.Add(new Box(100, 400, 80, 80, Color.White, 1.0f));
                boxlist.Add(new Box(200, 400, 80, 80, Color.White, 2.5f));
                boxlist.Add(new Box(300, 400, 80, 80, Color.White, 2.5f));
            }
        }

        private void GetText(IAsyncResult result)
        {
            string resultString = Guide.EndShowKeyboardInput(result);

            if (resultString.Length > 30)
            {
                resultString = resultString.Remove(30);
            }

            sblist.Clear();
            var userscore = new Score { Data = level.ToString(), Points = score, UserName = resultString };
            Mogade.SaveScore(MogadeHelper.LeaderboardId(Leaderboards.Main), userscore, ScoreResponseHandler);
            Mogade.GetRivals(MogadeHelper.LeaderboardId(Leaderboards.Main), LeaderboardScope.Overall, resultString, RivalResponseHandler);
            Mogade.GetLeaderboardCount(MogadeHelper.LeaderboardId(Leaderboards.Main), LeaderboardScope.Overall, TotalResponseHandler);
        }

        private void ScoreResponseHandler(Response<SavedScore> r)
        {
            //scoreboardRanks = string.Format("Daily Rank: {0}\nWeely Rank: {1}\nOverall Rank {2}", 0, 0, 0);
            if (!r.Success)
            {
                if (Guide.IsVisible == false)
                {
                    Guide.BeginShowMessageBox("Error", "Unable to retreive data from the server please check your network connection", new string[] { "OK" }, 0, MessageBoxIcon.Error, null, null);
                    return;
                }
            }
            else
            {
                scoreboardRanks = string.Format("Daily Rank: {0}\nWeely Rank: {1}\nOverall Rank {2}", r.Data.Ranks.Daily, r.Data.Ranks.Weekly, r.Data.Ranks.Overall);
            }
        }

        private void RivalResponseHandler(Response<IList<Score>> r)
        {
            //scoreboardRanks = string.Format("Daily Rank: {0}\nWeely Rank: {1}\nOverall Rank {2}", 0, 0, 0);
            if (!r.Success)
            {
                if (Guide.IsVisible == false)
                {
                    Guide.BeginShowMessageBox("Error", "Unable to retreive data from the server please check your network connection", new string[] { "OK" }, 0, MessageBoxIcon.Error, null, null);
                    return;
                }
                // Handle any errors here
            }
            else
            {
                for (var i = 0; i < 3; ++i)
                {
                    sblist.Add(new ScoreboardEntry(r.Data[i]));
                }
            }
        }

        private void TotalResponseHandler(Response<int> r)
        {
            if (!r.Success)
            {
                if (Guide.IsVisible == false)
                {
                    Guide.BeginShowMessageBox("Error", "Unable to retreive data from the server please check your network connection", new string[] { "OK" }, 0, MessageBoxIcon.Error, null, null);
                    return;
                }
                // Handle any errors here
            }
            else
            {
                totalscores = r.Data;
            }
        }

        private void LoadLeaderboard(LeaderboardScope scope, int page)
        {
            sblist.Clear();
            Mogade.GetLeaderboard(MogadeHelper.LeaderboardId(Leaderboards.Main), scope, page, r => LeaderboardReceived(r));
        }

        private void LeaderboardReceived(Response<LeaderboardScores> response)
        {
            if (!response.Success)
            {
                if (Guide.IsVisible == false)
                {
                    Guide.BeginShowMessageBox("Error", "Unable to retreive data from the server, please check your network connection", new string[] { "OK" }, 0, MessageBoxIcon.Error, null, null);
                    return;
                }
            }
            else
            {
                for (var i = 0; i < response.Data.Scores.Count; ++i)
                {
                    sblist.Add(new ScoreboardEntry(response.Data.Scores[i]));
                }
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using BrickBreaker;
using BrickBreaker.Screens;


namespace SuperSnakeGame.Screens
{
    public partial class GameScreenMulti : UserControl//TODO copy and arrange code
    {
        //added by daniel
        //player2 button control keys
        Boolean aKeyDown, sKeyDown, dKeyDown, wKeyDown, qKeyDown;

        #region global values

        //player1 button control keys - DO NOT CHANGE
        Boolean leftArrowDown, downArrowDown, rightArrowDown, upArrowDown, spaceDown;

        // Game values
        int lives;

        // Paddle and Ball objects
        Paddle paddle;
        Ball ball;

        // list of all blocks
        List<Block> blocks = new List<Block>();

        // Brushes
        SolidBrush paddleBrush = new SolidBrush(Color.White);
        SolidBrush ballBrush = new SolidBrush(Color.White);
        SolidBrush blockBrush = new SolidBrush(Color.Red);

        #endregion

        public GameScreenMulti()
        {
            InitializeComponent();
            OnStart();
        }

        public void daniel()
        {
            //added by daniel
            //set player 2 button presses to false
            aKeyDown = sKeyDown = dKeyDown = wKeyDown = false;
        }

        public void OnStart()
        {
            //set life counter
            lives = 3;

            //set all button presses to false.
            leftArrowDown = downArrowDown = rightArrowDown = upArrowDown = false;

            // setup starting paddle values and create paddle object
            int paddleWidth = 80;
            int paddleHeight = 20;
            int paddleX = ((this.Width / 2) - (paddleWidth / 2));
            int paddleY = (this.Height - paddleHeight) - 60;
            int paddleSpeed = 8;
            paddle = new Paddle(paddleX, paddleY, paddleWidth, paddleHeight, paddleSpeed, Color.White);

            // setup starting ball values
            int ballX = ((this.Width / 2) - 10);
            int ballY = (this.Height - paddle.height) - 80;

            // Creates a new ball
            int xSpeed = 6;
            int ySpeed = 6;
            int ballSize = 20;
            ball = new Ball(ballX, ballY, xSpeed, ySpeed, ballSize);

            // Creates blocks for generic level
            blocks.Clear();
            int x = 10;

            while (blocks.Count < 12)
            {
                x += 57;
                Block b1 = new Block(x, 10, 1, Color.White);
                blocks.Add(b1);
            }

            // start the game engine loop
            gameTimer.Enabled = true;
        }
        //TODO change to work for 2p
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            // Move the paddle
            if (leftArrowDown && paddle.x > 0)
            {
                paddle.Move("left");
            }
            if (rightArrowDown && paddle.x < (this.Width - paddle.width))
            {
                paddle.Move("right");
            }

            // Moves ball
            ball.Move();

            // Check for collision with top and side walls
            ball.WallCollision(this);

            // Check for collision of ball with paddle, (incl. paddle movement)
            ball.PaddleCollision(paddle, leftArrowDown, rightArrowDown);

            // Check if ball has collided with any blocks
            foreach (Block b in blocks)
            {
                if (ball.BlockCollision(b))
                {
                    blocks.Remove(b);

                    if (blocks.Count == 0)
                    {
                        gameTimer.Enabled = false;

                        OnEnd();
                    }

                    break;
                }
            }

            // Check for ball hitting bottom of screen
            if (ball.BottomCollision(this))
            {
                lives--;

                // Moves the ball back to origin
                ball.x = ((paddle.x - (ball.size / 2)) + (paddle.width / 2));
                ball.y = (this.Height - paddle.height) - 85;

                if (lives == 0)
                {
                    gameTimer.Enabled = false;

                    OnEnd();
                }
            }

            //redraw the screen
            Refresh();
        }

        private void GameScreen_KeyUp(object sender, KeyEventArgs e)
        {
            //player 1 button releases
            switch (e.KeyCode)
            {
                case Keys.Left:
                    leftArrowDown = false;
                    break;
                case Keys.Down:
                    downArrowDown = false;
                    break;
                case Keys.Right:
                    rightArrowDown = false;
                    break;
                case Keys.Up:
                    upArrowDown = false;
                    break;
                case Keys.Space:
                    spaceDown = false;
                    break;
                default:
                    break;
            }

            //added by daniel
            //player 2
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = true;
                    break;
                case Keys.S:
                    sKeyDown = true;
                    break;
                case Keys.D:
                    dKeyDown = true;
                    break;
                case Keys.W:
                    wKeyDown = true;
                    break;
                case Keys.Q:
                    qKeyDown = true;
                    break;
                default:
                    break;
            }
        }


        private void GameScreen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //player 1 button presses
            switch (e.KeyCode)
            {
                case Keys.Left:
                    leftArrowDown = true;
                    break;
                case Keys.Down:
                    downArrowDown = true;
                    break;
                case Keys.Right:
                    rightArrowDown = true;
                    break;
                case Keys.Up:
                    upArrowDown = true;
                    break;
                case Keys.Space:
                    spaceDown = true;
                    break;
                default:
                    break;
            }

            //added by daniel
            //player 2 button releases
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = false;
                    break;
                case Keys.S:
                    sKeyDown = false;
                    break;
                case Keys.D:
                    dKeyDown = false;
                    break;
                case Keys.W:
                    wKeyDown = false;
                    break;
                case Keys.Q:
                    qKeyDown = false;
                    break;
                default:
                    break;
            }

        }

        public void OnEnd()
        {
            // Goes to the game over screen
            Form form = this.FindForm();
            MenuScreen ps = new MenuScreen();

            ps.Location = new Point((form.Width - ps.Width) / 2, (form.Height - ps.Height) / 2);

            form.Controls.Add(ps);
            form.Controls.Remove(this);
        }

        public void GameScreen_Paint(object sender, PaintEventArgs e)
        {
            // Draws paddle
            e.Graphics.FillRectangle(paddleBrush, paddle.x, paddle.y, paddle.width, paddle.height);

            // Draws blocks
            foreach (Block b in blocks)
            {
                e.Graphics.FillRectangle(blockBrush, b.x, b.y, b.width, b.height);
            }

            // Draws balls
            e.Graphics.FillRectangle(ballBrush, ball.x, ball.y, ball.size, ball.size);
        }

    }
}

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

namespace TimeToFall
{

    public class ball
    {
        public string name;
        public double score;
        public int x;
        public int y;
        public int height;
        public int width;
        public Texture2D balltexture;
        public string texturename;
        public ball(int newx, int newy, int newheight, int newwidth, string newtexturename)
        {
            x = newx;
            y = newy;
            height = newheight;
            width = newwidth;
            texturename = newtexturename;
        }
        public void rysuj(SpriteBatch sprite)
        {
            sprite.Draw(balltexture, new Rectangle(x, y, width, height),Color.White);
        }
    }
    public class background
    {
        public Texture2D bgtexture;
        public string bgtexturename;
        public background(string newbgtexturename)
        {
            bgtexturename = newbgtexturename;
        }
        public void rysuj(SpriteBatch sprite)
        {
            sprite.Draw(bgtexture, new Vector2(0, 0), Color.White);
        }
    }
    public class sterowanie
    {
        public TouchLocation touch;
        public void update(TouchLocation nowakolekcja)
        {
            touch = nowakolekcja;
        }
       
        public int lewoczyprawo(TouchLocation tk)
        {
            if ((int)tk.Position.X > 246)
                return 10;
            else if ((int)tk.Position.X < 164)
                return -10;
            else
                return 0;
        }
        public bool czykolizja( ball b, przeszkoda p)
        {
            return (((b.y + b.height >= p.y + 1) && (b.y + b.height <= p.y + 10)) && ((b.x <= p.koniec1) || (b.x + b.width >= p.poczatek2)));
        }
    }
    public class przeszkoda
    {
        public int koniec1;
        public int poczatek2;
        public int height;
        public int y;
        public string texturename;
        int dziura;
        public przeszkoda()
        {
            texturename = "teksturaprzeszkody";
            height = 20;
            dziura = 100;
            y = 800 - height;
        }
        public void generuj_przeszkode(int s)
        {
            Random rnd = new Random(s);
            int losowa = rnd.Next(0, 480 - dziura);
            if ((losowa != 480 - dziura) && (losowa != 0))
            {
                koniec1 = losowa;
                poczatek2 = losowa + dziura;
            }
            else if (losowa == 480 - dziura)
            {
                koniec1 = 480 - dziura;
                poczatek2 = 0;
            }
            else
            {
                koniec1 = 0;
                poczatek2 = dziura;
            }
        }
        public void rysuj(SpriteBatch sprite, Texture2D texture)
        {
            sprite.Draw(texture, new Rectangle(0, y, koniec1, height), Color.White);
            sprite.Draw(texture, new Rectangle(poczatek2, y, 500-poczatek2, height), Color.White);
        }
        public void spry()
        {
            if (y <= 1) y = 800;
        }
    }
    public class otoczenie
    {
        public int szybkoscprzeszkod;
        public int szybkoscpilki;
        public otoczenie(int nsz, int nszp)
        {
            szybkoscpilki = nszp;
            szybkoscprzeszkod = nsz;
        }
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ball pilka1 = new ball(230, 0, 20, 20, "pilka1");
        background tlo1 = new background("bg");
        sterowanie sterowanie1 = new sterowanie();
        int liczbaklatek=0;
        przeszkoda przeszkoda1 = new przeszkoda();
        przeszkoda przeszkoda2 = new przeszkoda();
        przeszkoda przeszkoda3 = new przeszkoda();
        przeszkoda przeszkoda4 = new przeszkoda();
        otoczenie otoczenie1 =new otoczenie(4,4);



        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
            Content.RootDirectory = "Content";
            TargetElapsedTime = TimeSpan.FromTicks(333333);
            InactiveSleepTime = TimeSpan.FromSeconds(1);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            pilka1.balltexture = Content.Load<Texture2D>(pilka1.texturename);
            tlo1.bgtexture = Content.Load<Texture2D>(tlo1.bgtexturename);
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            TouchCollection touches = TouchPanel.GetState();
            if (touches.Count!=0) 
                pilka1.x += sterowanie1.lewoczyprawo(touches[touches.Count - 1]);
            if (pilka1.x>(480-pilka1.width)) 
                pilka1.x =(480-pilka1.width);
            if (pilka1.x<0)
                pilka1.x=0;
            // rozmkminka nad przeszkodami

            if (liczbaklatek == 0)
            {
                przeszkoda1.generuj_przeszkode(1);
                przeszkoda2.generuj_przeszkode(2);
                przeszkoda2.y += 200;
                przeszkoda3.generuj_przeszkode(3);
                przeszkoda3.y += 400;
                przeszkoda4.generuj_przeszkode(4);
                przeszkoda4.y += 600;
            }

            przeszkoda1.y -= otoczenie1.szybkoscprzeszkod; przeszkoda2.y -= otoczenie1.szybkoscprzeszkod; przeszkoda3.y -= otoczenie1.szybkoscprzeszkod; przeszkoda4.y -= otoczenie1.szybkoscprzeszkod;

            if ((sterowanie1.czykolizja(pilka1, przeszkoda1)) || (sterowanie1.czykolizja(pilka1, przeszkoda2)) || (sterowanie1.czykolizja(pilka1, przeszkoda3)) || (sterowanie1.czykolizja(pilka1, przeszkoda4)))
               pilka1.y -= otoczenie1.szybkoscpilki;
            else
               if (pilka1.y<=798-pilka1.height)pilka1.y += otoczenie1.szybkoscpilki;

            if (pilka1.y <= 1) pilka1.score = liczbaklatek / 100;

            przeszkoda1.spry(); przeszkoda2.spry(); przeszkoda3.spry(); przeszkoda4.spry();


            liczbaklatek++;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            tlo1.rysuj(spriteBatch);
            pilka1.rysuj(spriteBatch);

            przeszkoda1.rysuj(spriteBatch, Content.Load<Texture2D>(przeszkoda1.texturename));
            przeszkoda2.rysuj(spriteBatch, Content.Load<Texture2D>(przeszkoda1.texturename));
            przeszkoda3.rysuj(spriteBatch, Content.Load<Texture2D>(przeszkoda1.texturename));
            przeszkoda4.rysuj(spriteBatch, Content.Load<Texture2D>(przeszkoda1.texturename));  


            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

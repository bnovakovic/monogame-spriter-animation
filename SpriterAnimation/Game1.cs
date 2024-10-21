using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpriterDotNet;
using SpriterDotNet.MonoGame;
using SpriterDotNet.MonoGame.Sprites;
using SpriterDotNet.Providers;

namespace SpriterAnimation;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private MonoGameAnimator _animator;

    private static readonly Config SpriterConfig = new Config
    {
        MetadataEnabled = true,
        EventsEnabled = true,
        PoolingEnabled = true,
        TagsEnabled = true,
        VarsEnabled = true,
        SoundsEnabled = false
    };

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        const string characterSubDirector = "GreyGuy";
        var greyGuyRoot = Path.Combine(Content.RootDirectory, characterSubDirector);
        var scmlFilePath = Path.Combine(greyGuyRoot, "player.scml");
        var fileContent = File.ReadAllText(scmlFilePath);

        Spriter spriter = SpriterReader.Default.Read(fileContent);
        DefaultProviderFactory<ISprite, SoundEffect> factory = new DefaultProviderFactory<ISprite, SoundEffect>(SpriterConfig, true);

        SpriterEntity playerEntity = spriter.Entities.First();
        foreach (SpriterFolder spriterFolder in spriter.Folders)
        {
            foreach (SpriterFile spriterFile in spriterFolder.Files)
            {
                Texture2D texture = Content.Load<Texture2D>(characterSubDirector + "/" + spriterFile.Name.Replace(".png", ""));
                factory.SetSprite(spriter, spriterFolder, spriterFile, new TextureSprite(texture));
            }
        }
        
        Stack<SpriteDrawInfo> folderStack = new Stack<SpriteDrawInfo>();
        _animator = new MonoGameAnimator(playerEntity, factory, folderStack);
        _animator.Play("walk");
        _animator.Position = new Vector2(100.0f, 300.0f);
        _animator.Scale = Vector2.One;
        
        foreach (var playerEntityAnimation in playerEntity.Animations)
        {
            System.Console.WriteLine(playerEntityAnimation.Name);
        }
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        float deltaTime = gameTime.ElapsedGameTime.Ticks / (float)TimeSpan.TicksPerMillisecond;
        _animator.Update(deltaTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        _animator.Draw(_spriteBatch);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
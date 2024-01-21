using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
using System.Numerics;
using aengine_cs.aengine.windowing;
using aengine.core;
using aengine.graphics;
using aengine.ecs;
using Jitter.Dynamics;
using Jitter.Dynamics.Constraints;
using Jitter.Dynamics.Joints;
using Jitter.LinearMath;
using Sandbox.aengine;
using Sandbox.aengine.Gui;
using Console = aengine.core.Console;
using World = aengine.ecs.World;

namespace Sandbox; 

public static class Sandbox {
    public static unsafe async Task Main(string[] args) {
        // use only in ides like visual studio and rider,
        // the final build should have the assets folder in the same directory as the exe so remove this line below then
        Directory.SetCurrentDirectory("../../../");
        
        Window.create();
        Window.traceLogLevel = TraceLogLevel.LOG_NONE;
        Window.title = "aengine";
        SetWindowIcon(LoadImage("assets/logo.png"));

        var jump = LoadSound("assets/jump.mp3");
        var shoot = LoadSound("assets/pew.mp3");

        var albedo = LoadTexture("assets/albedo.png");
        var gun = LoadTexture("assets/hlpistol.png");
        var particle = LoadTexture("assets/particle.png");
        Gui.font = LoadFont("assets/fonts/font.ttf");

        var skybox = new[] {
            LoadTexture("assets/skybox/front.png"),
            LoadTexture("assets/skybox/back.png"),
            LoadTexture("assets/skybox/left.png"),
            LoadTexture("assets/skybox/right.png"),
            LoadTexture("assets/skybox/top.png"),
            LoadTexture("assets/skybox/bottom.png")
        };

        var camera = new Camera(Vector3.Zero, 90);
        float speed = 10;
        var isMouseLocked = false;

        World.camera = camera;

        // Entity player = Prefab.loadPrefab("assets/data/player.od", "player");
        Entity player = new Entity();

        var body = new Entity();
        body.transform.position.Y = 15;
        body.transform.scale = Vector3.One;
        body.addComponent(Prefab.loadComponent(body, "assets/data/player.od", "zombie_mesh"));
        body.addComponent(Prefab.loadComponent(body, "assets/data/player.od", "zombie"));

        var body2 = new Entity();
        body2.transform.position.Y = 15;
        body2.transform.position.X = 2.5f;
        body2.transform.scale = Vector3.One;
        body2.addComponent(new MeshComponent(body2, ShapeType.CYLINDER, YELLOW, new aTexture("assets/albedo.png")));
        body2.addComponent(new RigidBodyComponent(body2, 1, BodyType.DYNAMIC, ShapeType.CYLINDER));

        var window = new GuiWindow("SUIIIIIIIII", 10, 10, 300, 250);
        var textBox = new GuiTextBox();
        var slider = new GuiSlider();

        var ps = new ParticleSystem();
        var ps2 = new ParticleSystem();
        ps2.addComponent(new SpatialAudioComponent(ps2, new aSound("assets/at_dooms_gate.mp3")));

        var console = new Console();
 
        Prefab.loadScene("assets/data/estrada.od", "estrada");

        player = World.getEntity("playa");
        
        var light = new Entity();
        light.addComponent(Prefab.loadComponent(light, "assets/data/player.od", "light"));
        
        foreach (var entity in World.entities.Values)
            if (entity.hasComponent<MeshComponent>()) {
                if (entity.getComponent<MeshComponent>().isModel) {
                    entity.getComponent<MeshComponent>().setShader(light.getComponent<LightComponent>().shader);
                }
            }

        Entity water = Prefab.loadPrefab("assets/data/player.od", "water");
        
        Prefab.savePrefab("save_test.od", "SOME_SAVED_ENTITY", water);
        
        Prefab.saveScene("save_scene_test.od", "SOME_SAVED_SCENE");

        Entity slayer = new Entity("slayer");
        slayer.transform.position.X = 10;
        slayer.transform.scale = Vector3.One * 5;
        slayer.addComponent(new MeshComponent(slayer, new aModel("assets/models/cesium_man.m3d"), WHITE, new aTexture("assets/models/guytex.png")));
        
        ModelAnimation[] anims = LoadModelAnimations("assets/models/cesium_man.m3d");
        System.Console.WriteLine(anims.Length);
        int animFrameCounter = 0;

        // Main game loop
        while (Window.tick()) // Detect window close button or ESC key
        {
            World.update();

            if (IsKeyPressed(KeyboardKey.KEY_ESCAPE))
                isMouseLocked = !isMouseLocked;

            camera.setFirstPerson(0.1f, isMouseLocked);

            var velocity = Vector3.Zero;

            if (IsKeyDown(KeyboardKey.KEY_W))
                velocity += new Vector3((float)-Math.Sin(camera.rotation.Y * RayMath.DEG2RAD), 0,
                    (float)-Math.Cos(camera.rotation.Y * RayMath.DEG2RAD));
            if (IsKeyDown(KeyboardKey.KEY_S))
                velocity += new Vector3((float)Math.Sin(camera.rotation.Y * RayMath.DEG2RAD), 0,
                    (float)Math.Cos(camera.rotation.Y * RayMath.DEG2RAD));
            if (IsKeyDown(KeyboardKey.KEY_A))
                velocity += new Vector3((float)-Math.Cos(camera.rotation.Y * RayMath.DEG2RAD), 0,
                    (float)Math.Sin(camera.rotation.Y * RayMath.DEG2RAD));
            if (IsKeyDown(KeyboardKey.KEY_D))
                velocity += new Vector3((float)Math.Cos(camera.rotation.Y * RayMath.DEG2RAD), 0,
                    -(float)Math.Sin(camera.rotation.Y * RayMath.DEG2RAD));

            var speedMultiplier = IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT) ? 2.0f : 1.0f;
            velocity *= speed * speedMultiplier;

            if (IsKeyPressed(KeyboardKey.KEY_SPACE)) {
                PlaySound(jump);
                player.getComponent<RigidBodyComponent>().applyImpulse(0, 6, 0);
            }

            player.getComponent<RigidBodyComponent>().setLinearVelocity(velocity.X,
                player.getComponent<RigidBodyComponent>().body.LinearVelocity.Y, velocity.Z);

            if (player.transform.position.Y <= -500)
                player.getComponent<RigidBodyComponent>().setPosition(Vector3.UnitY * 5);

            player.getComponent<RigidBodyComponent>().setRotation(Vector3.Zero);
            camera.position = player.transform.position;
            camera.setDefaultFPSControls(speed, isMouseLocked, true);
            camera.defaultFpsMatrix();
            camera.update();

            foreach (var entity in World.entities.Values) {
                if (entity.hasComponent<MeshComponent>()) {
                    if (!entity.getComponent<MeshComponent>().isModel)
                        entity.transform.rotation.X =
                            (float)Math.Atan2(player.transform.position.X - entity.transform.position.X,
                                player.transform.position.Z - entity.transform.position.Z) * RayMath.RAD2DEG;
                    if (entity.hasComponent<RigidBodyComponent>())
                        if (camera.raycast.isColliding(entity.transform,
                                entity.getComponent<RigidBodyComponent>().shapeType) &&
                            IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                            if (!entity.getComponent<RigidBodyComponent>().body.IsStatic && isMouseLocked)
                                entity.getComponent<RigidBodyComponent>().applyImpulse(camera.raycast.target / 5);
                }

                if (entity.hasComponent<FluidComponent>()) {
                    if (player.getAABB().overlaps(entity.getAABB())) {
                        if (entity.hasComponent<SpatialAudioComponent>()) {
                            entity.getComponent<SpatialAudioComponent>().play();
                            entity.getComponent<SpatialAudioComponent>().canPlay = false;
                        }
                    }
                    else {
                        entity.getComponent<SpatialAudioComponent>().canPlay = true;
                    }
                }
            }

            if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && isMouseLocked) {
                PlaySound(shoot);
                velocity.X = -camera.raycast.target.X * 0.2f;
                velocity.Z = -camera.raycast.target.Z * 0.2f;
                player.getComponent<RigidBodyComponent>().applyImpulse(-camera.raycast.target * 0.25f);
            }

            if (IsKeyPressed(KeyboardKey.KEY_GRAVE)) {
                console.window.setPosition(10, 10);
                console.toggleActive();
            }

            var p = new ParticleComponent(new ParticleBehaviour(false, 5f), WHITE, particle, Vector2.One / 2, 25);
            p.addBehaviour(new DecayBehaviour());

            var p2 = new ParticleComponent(new RandomSideBehaviour(-5f), RED, 20);
            p2.addBehaviour(new GradientBehaviour(p2.color, BLACK, 150));
            p2.addBehaviour(new DecayBehaviour());
            p2.addBehaviour(new ScaleBehaviour(true));

            ps.addParticle(p, ParticleSpawn.sphere(4), 2);
            ps2.addParticle(p2, ParticleSpawn.cube(8, 0, 0));

            ps2.getComponent<SpatialAudioComponent>().play();
            
            // animation
            if (IsKeyDown(KeyboardKey.KEY_ENTER)) {
                animFrameCounter += (int)(150 * GetFrameTime());
                UpdateModelAnimation(slayer.getComponent<MeshComponent>().model.data, anims[0], animFrameCounter);
                if (animFrameCounter >= anims[0].frameCount) animFrameCounter = 0;
            }
            
            Window.beginRender();
            ClearBackground(SKYBLUE);

            BeginMode3D(camera.matrix);
            Rendering.drawSkyBox(skybox, WHITE, 1000);
            World.render();

            Rendering.drawDebugAxies();
            Rendering.drawArrow(Vector3.Zero, Vector3.One, GREEN);
            
            EndMode3D();
            Rendering.drawCrosshair(WHITE);
            DrawTexturePro(gun, new Rectangle(0, 0, gun.width, gun.height),
                new Rectangle(Window.renderWidth / 2 + 75, Window.renderHeight - 250, 200, 250), Vector2.Zero, 0, WHITE);

            window.render();

            if (window.active) {
                Gui.GuiTextPro(Gui.font, "FPS: " + GetFPS(), new Vector2(10, 10), Gui.font.baseSize, WHITE, window);
                Gui.GuiTextPro(Gui.font, "Entities: " + World.entities.Count, new Vector2(10, 50), Gui.font.baseSize, WHITE,
                    window);

                if (Gui.GuiButton("Render Colliders", 10, 100, 240, 40, window, TextPositioning.LEFT))
                    World.renderColliders = !World.renderColliders;

                textBox.render(10, 150, 240, 40, window);
                slider.render(10, 200, 240, 40, window);
            }

            console.render();

            Window.endRender();
        }

        World.dispose();
        CloseWindow();
    }
}
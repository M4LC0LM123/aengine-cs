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
    public static async Task Main(string[] args) {
        // use only in ides like visual studio and rider,
        // the final build should have the assets folder in the same directory as the exe so remove this line below then
        Directory.SetCurrentDirectory("../../../");
        
        Window.create();
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

        var camera = new Camera(new Vector3(0, 5, 0), 90);
        float speed = 10;
        var isMouseLocked = false;

        World.camera = camera;

        var player = new Entity();
        player.transform.position = camera.position;
        player.transform.scale = Vector3.One;
        player.tag = "playa";
        player.addComponent(new RigidBodyComponent(player, 1.0f, BodyType.DYNAMIC, ShapeType.SPHERE));

        var body = new Entity();
        body.transform.position.Y = 15;
        body.transform.scale = Vector3.One;
        body.addComponent(new MeshComponent(body, GREEN, albedo));
        body.addComponent(new RigidBodyComponent(body));

        var body2 = new Entity();
        body2.transform.position.Y = 15;
        body2.transform.position.X = 2.5f;
        body2.transform.scale = Vector3.One;
        body2.addComponent(new MeshComponent(body2, GenMeshCylinder(1, 1, 15), YELLOW, albedo));
        body2.addComponent(new RigidBodyComponent(body2, 1, BodyType.DYNAMIC, ShapeType.CYLINDER));

        var dummy = new Dummy();

        var window = new GuiWindow("SUIIIIIIIII", 10, 10, 300, 250);
        var textBox = new GuiTextBox();
        var slider = new GuiSlider();

        var ps = new ParticleSystem();
        var ps2 = new ParticleSystem();
        ps2.addComponent(new SpatialAudioComponent(ps2, LoadSound("assets/at_dooms_gate.mp3")));

        var console = new Console();
 
        var scenePrefab = new ScenePrefab("assets/maps/map3.json");

        foreach (var obj in scenePrefab.data)
            switch (obj.id) {
                case 0:
                    var cube = new Entity();
                    cube.transform.position = new Vector3(obj.x, obj.y, obj.z);
                    cube.transform.scale = new Vector3(obj.w, obj.h, obj.d);
                    cube.transform.rotation = new Vector3(obj.rx, obj.ry, obj.rz);
                    cube.addComponent(new MeshComponent(cube, Rendering.getRandomColor(), albedo));
                    cube.addComponent(new RigidBodyComponent(cube));
                    break;
                case 1:
                    var sphere = new Entity();
                    sphere.transform.position = new Vector3(obj.x, obj.y, obj.z);
                    sphere.transform.scale = new Vector3(obj.w, obj.h, obj.d);
                    sphere.transform.rotation = new Vector3(obj.rx, obj.ry, obj.rz);
                    sphere.addComponent(new MeshComponent(sphere, GenMeshSphere(sphere.transform.scale.X, 15, 15),
                        Rendering.getRandomColor(), albedo));
                    sphere.addComponent(new RigidBodyComponent(sphere, 1, BodyType.DYNAMIC, ShapeType.SPHERE));
                    break;
                case 2:
                    var wall = new Entity();
                    wall.transform.position = new Vector3(obj.x, obj.y, obj.z);
                    wall.transform.scale = new Vector3(obj.w, obj.h, obj.d);
                    wall.transform.rotation = new Vector3(obj.rx, obj.ry, obj.rz);
                    wall.addComponent(new MeshComponent(wall, Rendering.getRandomColor(), albedo));
                    wall.addComponent(new RigidBodyComponent(wall, 1.0f, BodyType.STATIC));
                    break;
                case 3:
                    var model = new Entity();
                    model.transform.position = new Vector3(obj.x, obj.y, obj.z);
                    model.transform.scale = new Vector3(obj.w, obj.h, obj.d);
                    model.transform.rotation = new Vector3(obj.rx, obj.ry, obj.rz);
                    var mesh = new MeshComponent(model, LoadModel("assets/models/zombie.glb"), WHITE, new Texture());
                    mesh.scale = 5;
                    model.addComponent(mesh);
                    break;
                case 4:
                    var hehe = new Entity();
                    hehe.tag = "hehe";
                    hehe.transform.position = new Vector3(obj.x, obj.y, obj.z);
                    hehe.transform.scale = new Vector3(obj.w, obj.h, obj.d);
                    hehe.transform.rotation = new Vector3(obj.rx, obj.ry, obj.rz);
                    hehe.addComponent(new MeshComponent(hehe, WHITE, LoadTexture("assets/trollface.png"), false));
                    break;
                case 5:
                    var billBoard = new Entity();
                    billBoard.transform.position = new Vector3(obj.x, obj.y, obj.z);
                    billBoard.transform.scale = new Vector3(obj.w, obj.h, obj.d);
                    billBoard.transform.rotation = new Vector3(obj.rx, obj.ry, obj.rz);
                    billBoard.addComponent(new MeshComponent(billBoard, WHITE, LoadTexture("assets/cacodemon.png"),
                        false));
                    break;
                case 6:
                    ps.setFromSceneObj(obj);
                    break;
                case 7:
                    ps2.setFromSceneObj(obj);
                    break;
                case 8:
                    dummy.setPosition(new Vector3(obj.x, obj.y, obj.z));
                    break;
                case 9:
                    var fluid = new Entity();
                    fluid.setFromSceneObj(obj);
                    fluid.addComponent(new FluidComponent(fluid, new aShader(null, "assets/shaders/wave.frag"),
                        LoadTexture("assets/water.png"), new Color(255, 255, 255, 125)));
                    fluid.addComponent(new SpatialAudioComponent(fluid, LoadSound("assets/splash.wav")));
                    break;
                case 10:
                    var cone = new Entity();
                    cone.setFromSceneObj(obj);
                    cone.addComponent(new MeshComponent(cone,
                        GenMeshCone(cone.transform.scale.X, cone.transform.scale.Y, 15), Rendering.getRandomColor(),
                        albedo));
                    cone.addComponent(new RigidBodyComponent(cone, 1.0f, BodyType.STATIC, ShapeType.CONE));
                    break;
                case 11:
                    var terrain = new Entity();
                    terrain.setFromSceneObj(obj);
                    Image heightmap = LoadImage("assets/heightmap2.png");
                    terrain.addComponent(new MeshComponent(terrain,
                        GenMeshHeightmap(heightmap, terrain.transform.scale), GREEN, albedo));
                    terrain.addComponent(new RigidBodyComponent(terrain, LoadTextureFromImage(heightmap), 1, BodyType.STATIC));
                    break;
                case 12:
                    var light = new Entity();
                    light.setFromSceneObj(obj);
                    light.addComponent(new LightComponent(light,
                        new aShader("assets/shaders/light.vert", "assets/shaders/light.frag"), WHITE,
                        LightType.POINT));
                    
                    foreach (var entity in World.entities)
                        if (entity.hasComponent<MeshComponent>()) {
                            if (entity.getComponent<MeshComponent>().isModel) {
                                entity.getComponent<MeshComponent>().setShader(light.getComponent<LightComponent>().shader);
                            }
                        }
                    break;
            }
        
        // Main game loop
        while (!WindowShouldClose()) // Detect window close button or ESC key
        {
            Window.tick();
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

            player.getComponent<RigidBodyComponent>().body.Orientation =
                JMatrix.CreateFromQuaternion(new JQuaternion());
            camera.position = player.transform.position;
            camera.setDefaultFPSControls(speed, isMouseLocked, true);
            camera.defaultFpsMatrix();
            camera.update();

            foreach (var entity in World.entities) {
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
                velocity.X = -camera.raycast.target.X / 5;
                velocity.Z = -camera.raycast.target.Z / 5;
                player.getComponent<RigidBodyComponent>().applyImpulse(-camera.raycast.target / 5);
            }

            if (IsKeyPressed(KeyboardKey.KEY_GRAVE)) {
                console.window.setPosition(10, 10);
                console.active = !console.active;
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

            dummy.transform.rotation.Y += 100 * GetFrameTime();

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

            Gui.GuiTextPro(Gui.font, "FPS: " + GetFPS(), new Vector2(10, 10), Gui.font.baseSize, WHITE, window);
            Gui.GuiTextPro(Gui.font, "Entities: " + World.entities.Count, new Vector2(10, 50), Gui.font.baseSize, WHITE,
                window);

            if (Gui.GuiButton("Render Colliders", 10, 100, 240, 40, window, TextPositioning.LEFT))
                World.RenderColliders = !World.RenderColliders;

            textBox.render(10, 150, 240, 40, window);
            slider.render(10, 200, 240, 40, window);

            console.render();

            Window.endRender();
        }

        World.dispose();
        CloseWindow();
    }
}
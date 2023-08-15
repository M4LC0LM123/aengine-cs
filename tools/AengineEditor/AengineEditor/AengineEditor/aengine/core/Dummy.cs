using System.Numerics;
using aengine.ecs;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.Dynamics.Constraints;
using Jitter.Dynamics.Constraints.SingleBody;
using Jitter.LinearMath;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
using PointOnPoint = Jitter.Dynamics.Constraints.PointOnPoint;

namespace aengine.core;

public class Dummy : Entity
{
    public Model head;
    public Model torso;
    public Model leftArm;
    public Model rightArm;
    public Model leftLeg;
    public Model rightLeg;
    
    public float scale;

    public Shape limbShape;
    public Shape headShape;
    public Shape torsoShape;
    
    public RigidBody r_head;
    public RigidBody r_torso;
    public RigidBody r_leftArm;
    public RigidBody r_rightArm;
    public RigidBody r_leftLeg;
    public RigidBody r_rightLeg;

    public Constraint c_head;
    public Constraint c_leftArm;
    public Constraint c_rightArm;
    public Constraint c_leftLeg;
    public Constraint c_rightLeg;
    
    public Dummy()
    {
        scale = 1;

        limbShape = new BoxShape(scale / 2, scale + 0.5f, scale / 2);
        headShape = new SphereShape(scale / 2);
        torsoShape = new BoxShape(scale, scale + 0.5f, scale / 2);

        r_head = new RigidBody(headShape);
        r_torso = new RigidBody(torsoShape);
        r_leftArm = new RigidBody(limbShape);
        r_rightArm = new RigidBody(limbShape);
        r_leftLeg = new RigidBody(limbShape);
        r_rightLeg = new RigidBody(limbShape);

        r_head.Position = new JVector(transform.position.X, transform.position.Y + scale + 0.25f, transform.position.Z);
        r_torso.Position = new JVector(transform.position.X, transform.position.Y, transform.position.Z);
        r_leftArm.Position = new JVector(transform.position.X - scale * 0.75f, transform.position.Y, transform.position.Z);
        r_rightArm.Position = new JVector(transform.position.X + scale * 0.75f, transform.position.Y, transform.position.Z);
        r_leftLeg.Position = new JVector(transform.position.X - scale/4, transform.position.Y - (scale + 0.5f), transform.position.Z);
        r_rightLeg.Position = new JVector(transform.position.X + scale / 4, transform.position.Y - (scale + 0.5f), transform.position.Z);
        
        World.world.AddBody(r_head);
        World.world.AddBody(r_torso);
        World.world.AddBody(r_leftArm);
        World.world.AddBody(r_rightArm);
        World.world.AddBody(r_leftLeg);
        World.world.AddBody(r_rightLeg);

        c_head = new PointPointDistance(r_head, r_torso, r_head.Position, r_torso.Position);
        c_leftArm = new PointPointDistance(r_leftArm, r_torso, r_leftArm.Position, r_torso.Position);
        c_rightArm = new PointPointDistance(r_rightArm, r_torso, r_rightArm.Position, r_torso.Position);
        c_leftLeg = new PointPointDistance(r_leftLeg, r_torso, r_leftLeg.Position, r_torso.Position);
        c_rightLeg = new PointPointDistance(r_rightLeg, r_torso, r_rightLeg.Position, r_torso.Position);

        World.world.AddConstraint(c_head);
        World.world.AddConstraint(c_leftArm);
        World.world.AddConstraint(c_rightArm);
        World.world.AddConstraint(c_leftLeg);
        World.world.AddConstraint(c_rightLeg);

        // head = LoadModelFromMesh(GenMeshCube(scale, scale, scale));
        head = LoadModelFromMesh(GenMeshSphere(scale/2, 15, 15));
        torso = LoadModelFromMesh(GenMeshCube(scale, scale + 0.5f, scale / 2));
        leftArm = LoadModelFromMesh(GenMeshCube(scale / 2, scale + 0.5f, scale / 2));
        rightArm = LoadModelFromMesh(GenMeshCube(scale / 2, scale + 0.5f, scale / 2));
        leftLeg = LoadModelFromMesh(GenMeshCube(scale / 2, scale + 0.5f, scale / 2));
        rightLeg = LoadModelFromMesh(GenMeshCube(scale / 2, scale + 0.5f, scale / 2));
    }

    public virtual void setPosition(Vector3 position)
    {
        r_head.Position = new JVector(position.X, position.Y + scale + 0.25f,position.Z);
        r_torso.Position = new JVector(position.X,position.Y,position.Z);
        r_leftArm.Position = new JVector(position.X - scale * 0.75f,position.Y,position.Z);
        r_rightArm.Position = new JVector(position.X + scale * 0.75f,position.Y,position.Z);
        r_leftLeg.Position = new JVector(position.X - scale/4, position.Y - (scale + 0.5f),position.Z);
        r_rightLeg.Position = new JVector(position.X + scale / 4, position.Y - (scale + 0.5f),position.Z);
    }

    public override void update()
    {
        base.update();
        transform.position = new Vector3(r_torso.Position.X, r_torso.Position.Y, r_torso.Position.Z);

        // r_head.Orientation = JMatrix.CreateFromYawPitchRoll(0, 0, 0);
        // r_torso.Orientation = JMatrix.CreateFromYawPitchRoll(0, 0, 0);
        // r_leftArm.Orientation = JMatrix.CreateFromYawPitchRoll(0, 0, 0);
        // r_rightArm.Orientation = JMatrix.CreateFromYawPitchRoll(0, 0, 0);
        // r_leftLeg.Orientation = JMatrix.CreateFromYawPitchRoll(0, 0, 0);
        // r_rightLeg.Orientation = JMatrix.CreateFromYawPitchRoll(0, 0, 0);

        head.transform = RayMath.MatrixRotateXYZ(core.aengine.MatrixToEuler(r_head.Orientation));
        torso.transform = RayMath.MatrixRotateXYZ(core.aengine.MatrixToEuler(r_torso.Orientation));
        leftArm.transform = RayMath.MatrixRotateXYZ(core.aengine.MatrixToEuler(r_leftArm.Orientation));
        rightArm.transform = RayMath.MatrixRotateXYZ(core.aengine.MatrixToEuler(r_rightArm.Orientation));
        leftLeg.transform = RayMath.MatrixRotateXYZ(core.aengine.MatrixToEuler(r_leftLeg.Orientation));
        rightLeg.transform = RayMath.MatrixRotateXYZ(core.aengine.MatrixToEuler(r_rightLeg.Orientation));
    }

    public override void render()
    {
        base.render();
        DrawModel(head, new Vector3(r_head.Position.X, r_head.Position.Y, r_head.Position.Z), scale, WHITE);
        DrawModel(torso, new Vector3(r_torso.Position.X, r_torso.Position.Y, r_torso.Position.Z), scale, WHITE);
        DrawModel(leftArm, new Vector3(r_leftArm.Position.X, r_leftArm.Position.Y, r_leftArm.Position.Z), scale, WHITE);
        DrawModel(rightArm, new Vector3(r_rightArm.Position.X, r_rightArm.Position.Y, r_rightArm.Position.Z), scale, WHITE);
        DrawModel(leftLeg, new Vector3(r_leftLeg.Position.X, r_leftLeg.Position.Y, r_leftLeg.Position.Z), scale, WHITE);
        DrawModel(rightLeg, new Vector3(r_rightLeg.Position.X, r_rightLeg.Position.Y, r_rightLeg.Position.Z), scale, WHITE);

        DrawModelWires(head, new Vector3(r_head.Position.X, r_head.Position.Y, r_head.Position.Z), scale, BLACK);
        DrawModelWires(torso, new Vector3(r_torso.Position.X, r_torso.Position.Y, r_torso.Position.Z), scale, BLACK);
        DrawModelWires(leftArm, new Vector3(r_leftArm.Position.X, r_leftArm.Position.Y, r_leftArm.Position.Z), scale, BLACK);
        DrawModelWires(rightArm, new Vector3(r_rightArm.Position.X, r_rightArm.Position.Y, r_rightArm.Position.Z), scale, BLACK);
        DrawModelWires(leftLeg, new Vector3(r_leftLeg.Position.X, r_leftLeg.Position.Y, r_leftLeg.Position.Z), scale, BLACK);
        DrawModelWires(rightLeg, new Vector3(r_rightLeg.Position.X, r_rightLeg.Position.Y, r_rightLeg.Position.Z), scale, BLACK);
    }

    public override void dispose()
    {
        base.dispose();
    }
}
using System.Numerics;
using OpenTK.Graphics.OpenGL;

namespace aengine.graphics; 

public class Light {
    public Vector3 position;
    public float intensity;
    
    public Vector4 ambient;
    public Vector4 diffuse;
    public Vector4 specular;

    private float[] m_position;
    private float[] m_ambient;
    private float[] m_diffuse;
    private float[] m_specular;
    
    public Light() {
        position = Vector3.Zero;
        intensity = 2.0f;
        
        m_position = new[] { position.X, position.Y, position.Z, 2.0f };
        
        ambient = Vector4.Zero with { X = 0.2f, Y = 0.2f, Z = 0.2f, W = 1.0f };
        diffuse = Vector4.Zero with { X = 0.8f, Y = 0.8f, Z = 0.8f, W = 1.0f };
        specular = Vector4.Zero with { X = 1.0f, Y = 1.0f, Z = 1.0f, W = 1.0f};
        
        m_ambient = new []{ ambient.X, ambient.Y, ambient.Z, ambient.W };
        m_diffuse = new []{ diffuse.X, diffuse.Y, diffuse.Z, diffuse.W };
        m_specular = new []{ specular.X, specular.Y, specular.Z, specular.W };
    }

    public Light(Vector3 position, Vector4 ambient, Vector4 diffuse, Vector4 specular, float intensity) {
        this.position = position;
        this.intensity = intensity;
        
        m_position = new[] { this.position.X, this.position.Y, this.position.Z, 2.0f };
        
        this.ambient = ambient;
        this.diffuse = diffuse;
        this.specular = specular;
        
        m_ambient = new []{ this.ambient.X, this.ambient.Y, this.ambient.Z, this.ambient.W };
        m_diffuse = new []{ this.diffuse.X, this.diffuse.Y, this.diffuse.Z, this.diffuse.W };
        m_specular = new []{ this.specular.X, this.specular.Y, this.specular.Z, this.specular.W };
    }

    private void update() {
        m_position[0] = position.X;
        m_position[1] = position.Y;
        m_position[2] = position.Z;
        m_position[3] = intensity;
        
        m_ambient[0] = ambient.X;
        m_ambient[1] = ambient.Y;
        m_ambient[2] = ambient.Z;
        m_ambient[3] = ambient.W;
        
        m_diffuse[0] = diffuse.X;
        m_diffuse[1] = diffuse.Y;
        m_diffuse[2] = diffuse.Z;
        m_diffuse[3] = diffuse.W;
        
        m_specular[0] = specular.X;
        m_specular[1] = specular.Y;
        m_specular[2] = specular.Z;
        m_specular[3] = specular.W;
    }

    public void enable(LightName lightIndex, ShadingModel shadingModel = ShadingModel.Smooth) {
        update();
        
        GL.Enable((EnableCap)lightIndex);
        GL.ShadeModel(shadingModel);
        
        GL.Light(lightIndex, LightParameter.Position, m_position);
        GL.Light(lightIndex, LightParameter.Ambient, m_ambient);
        GL.Light(lightIndex, LightParameter.Diffuse, m_diffuse);
        GL.Light(lightIndex, LightParameter.Specular, m_specular);
    }
    
}
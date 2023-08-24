using System.Numerics;
using OpenTK.Graphics.OpenGL;

namespace aengine.graphics; 

public class Material {
    public Vector4 ambient;
    public Vector4 diffuse;
    public Vector4 specular;
    public float shininess;

    private float[] m_ambient;
    private float[] m_diffuse;
    private float[] m_specular;
    
    public Material() {
        ambient = Vector4.Zero with { X = 0.2f, Y = 0.2f, Z = 0.2f, W = 1.0f };
        diffuse = Vector4.Zero with { X = 0.8f, Y = 0.8f, Z = 0.8f, W = 1.0f };
        specular = Vector4.Zero with { X = 1.0f, Y = 1.0f, Z = 1.0f, W = 1.0f};
        shininess = 0.0f; // crank it up so it gets shinier e.g. shininess = 64.0f;
        
        m_ambient = new []{ ambient.X, ambient.Y, ambient.Z, ambient.W };
        m_diffuse = new []{ diffuse.X, diffuse.Y, diffuse.Z, diffuse.W };
        m_specular = new []{ specular.X, specular.Y, specular.Z, specular.W };
    }

    public Material(Vector4 ambient, Vector4 diffuse, Vector4 specular, float shininess) {
        this.ambient = ambient;
        this.diffuse = diffuse;
        this.specular = specular;
        this.shininess = shininess;
        
        m_ambient = new []{ this.ambient.X, this.ambient.Y, this.ambient.Z, this.ambient.W };
        m_diffuse = new []{ this.diffuse.X, this.diffuse.Y, this.diffuse.Z, this.diffuse.W };
        m_specular = new []{ this.specular.X, this.specular.Y, this.specular.Z, this.specular.W };
    }

    private void update() {
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
    
    public void enable(MaterialFace face = MaterialFace.Front) {
        update();
        
        GL.Material(face, MaterialParameter.Ambient, m_ambient);
        GL.Material(face, MaterialParameter.Diffuse, m_diffuse);
        GL.Material(face, MaterialParameter.Specular, m_specular);
        GL.Material(face, MaterialParameter.Shininess, shininess);
    }
    
}
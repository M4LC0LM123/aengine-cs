using System.Numerics;
using Raylib_CsLo;

namespace aengine.ecs {
    public class Entity {
        private Entity m_parent;
        private List<Entity> m_children;
        private TransformComponent m_rootTransform;

        public int id;
        public String tag;
        public TransformComponent transform;
        public List<Component> components;

        public bool selected = false; // editor stuff

        public Entity() {
            m_parent = null;

            id = World.entities.Count;
            tag = "Entity" + id;
            transform = new TransformComponent(this);
            m_rootTransform = transform;
            components = new List<Component>();
            
            if (World.hasTag(tag)) {
                tag += id;
            }
            
            World.entities.Add(tag, this);
        }

        public Entity(string tag) {
            id = World.entities.Count;
            this.tag = tag;
            transform = new TransformComponent(this);
            m_rootTransform = transform;
            components = new List<Component>();
            
            if (World.hasTag(tag)) {
                tag += id;
            }
            
            World.entities.Add(tag, this);
        }

        public virtual void setParent<T>(T parent) where T : Entity {
            m_parent = parent;
        }

        public virtual Entity getParent() {
            return m_parent;
        }

        public virtual void addChild<T>(T child) where T : Entity {
            m_children.Add(child);    
        }

        public virtual List<Entity> getChildren() {
            return m_children;
        }

        public virtual void setRootTransform() {
            m_rootTransform = transform;
        }

        public virtual Entity getChild(int id) {
            for (int i = 0; i < m_children.Count; i++) {
                if (m_children[i].id == id) {
                    return m_children[i];
                }
            }

            return null;
        }
        
        public virtual Entity getChild(string tag) {
            for (int i = 0; i < m_children.Count; i++) {
                if (m_children[i].tag == tag) {
                    return m_children[i];
                }
            }

            return null;
        }
        
        public virtual T addComponent<T>(params object[] args) where T : Component {
            T component = (T)Activator.CreateInstance(typeof(T), args);
            components.Add(component);
            return component;
        }

        public virtual void addComponent<T>(T component) where T : Component {
            components.Add(component);
        }

        public virtual T? getComponent<T>() where T : Component {
            foreach (Component component in components) {
                if (component.GetType().Equals(typeof(T))) {
                    return (T)component;
                }
            }

            return default;
        }

        public virtual bool hasComponent<T>() where T : Component {
            foreach (Component component in components) {
                if (component.GetType().Equals(typeof(T))) {
                    return true;
                }
            }

            return false;
        }

        public virtual void setFromSceneObj(SceneObject obj) {
            transform.position = new Vector3(obj.x, obj.y, obj.z);
            transform.scale = new Vector3(obj.w, obj.h, obj.d);
            transform.rotation = new Vector3(obj.rx, obj.ry, obj.rz);
        }

        public virtual BoundingBox getBoundingBox() {
            return new BoundingBox(RayMath.Vector3Subtract(transform.position, transform.scale / 2), transform.scale);
        }

        public virtual AABB getAABB() {
            return new AABB(transform.position.X - transform.scale.X / 2, transform.position.Y - transform.scale.Y / 2,
                transform.position.Z - transform.scale.Z / 2, transform.scale.X, transform.scale.Y, transform.scale.Z);
        }

        public virtual void update() {
            foreach (Component component in components) {
                component.update(this);
            }
            
            if (m_parent != null) {
                transform.position = m_rootTransform.position + m_parent.transform.position;
                transform.scale = m_rootTransform.scale + m_parent.transform.scale;
                transform.rotation = m_rootTransform.rotation + m_parent.transform.rotation;
            }
        }

        public virtual void render() {
            foreach (Component component in components) {
                component.render();
            }
        }

        public virtual void dispose() {
            foreach (Component component in components) {
                component.dispose();
            }
        }
    }
}
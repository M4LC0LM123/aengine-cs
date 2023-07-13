using System.Numerics;

namespace aengine.ecs
{
    public class Entity 
    {
        public int id;
        public String tag;
        public TransformComponent transform;
        public List<Component> components;
        
        public Entity()
        {
            this.id = World.entities.Count;
            this.tag = "Entity" + World.entities.Count.ToString();
            transform = new TransformComponent(this);
            this.components = new List<Component>();
            World.entities.Add(this);
        }

        public virtual T addComponent<T>(params object[] args) where T : Component
        {
            T component = (T)Activator.CreateInstance(typeof(T), args);
            components.Add(component);
            return component;
        }

        public virtual void addComponent<T>(T component) where T : Component
        {
            components.Add(component);
        }

        public virtual T? getComponent<T>() where T : Component
        {
            foreach (Component component in components)
            {
                if (component.GetType().Equals(typeof(T)))
                {
                    return (T)component;
                }
            }
            return null;
        }

        public virtual bool hasComponent<T>() where T : Component
        {
            foreach (Component component in this.components)
            {
                if (component.GetType().Equals(typeof(T)))
                {
                    return true;
                }
            }
            return false;
        }


        public virtual void update()
        {
            foreach (Component component in this.components)
            {
                component.update(this);
            }
        }

        public virtual void render()
        {
            foreach (Component component in this.components)
            {
                component.render();
            }
        }

        public virtual void dispose()
        {
            foreach (Component component in this.components)
            {
                component.dispose();
            }
        }

    }
}
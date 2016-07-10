using System;
using System.Linq;
using Unity.Linq;
using UnityEngine;

namespace Assets.Dungeon.Scripts.Helpers
{
    public static class ComponenHelpers
    {
        public static Animation FindAnimationComponent(this GameObject gameObject) 
        {
            try
            {
                var gameObjects = gameObject.DescendantsAndSelf().ToList();
                var componentHolder =
                    gameObjects.FirstOrDefault(_ => _.GetComponent<Animation>() != null);
                if (componentHolder != null)
                    return componentHolder.GetComponent<Animation>();
                Debug.LogErrorFormat("Object {0} doesn't contains {1} component on itself or any descendant",
                    gameObject.name, typeof(Animation));
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("Object {0} doesn't contains {1} component on itself or any descendant: {2}",
                    gameObject.name, typeof(Animation), ex);
                return null;
            }
        }
        public static NavMeshAgent FindNavMeshAgentComponent(this GameObject gameObject) 
        {
            try
            {
                var gameObjects = gameObject.DescendantsAndSelf().ToList();
                var componentHolder =
                    gameObjects.FirstOrDefault(_ => _.GetComponent<NavMeshAgent>() != null);
                if (componentHolder != null)
                    return componentHolder.GetComponent<NavMeshAgent>();
                Debug.LogErrorFormat("Object {0} doesn't contains {1} component on itself or any descendant",
                    gameObject.name, typeof(NavMeshAgent));
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("Object {0} doesn't contains {1} component on itself or any descendant: {2}",
                    gameObject.name, typeof(NavMeshAgent), ex);
                return null;
            }
        }
        public static NavMeshObstacle FindNavMeshObstacleComponent(this GameObject gameObject) 
        {
            try
            {
                var gameObjects = gameObject.DescendantsAndSelf().ToList();
                var componentHolder =
                    gameObjects.FirstOrDefault(_ => _.GetComponent<NavMeshObstacle>() != null);
                if (componentHolder != null)
                    return componentHolder.GetComponent<NavMeshObstacle>();
                Debug.LogErrorFormat("Object {0} doesn't contains {1} component on itself or any descendant",
                    gameObject.name, typeof(NavMeshObstacle));
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("Object {0} doesn't contains {1} component on itself or any descendant: {2}",
                    gameObject.name, typeof(NavMeshObstacle), ex);
                return null;
            }
        }
    }
}
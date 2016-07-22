using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Game.Scripts.Battle.Presenter;
using TMPro;
using Unity.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Assets.Game.Scripts.Helpers {
    public static class ComponenHelpers {
        public static List<Transform> GetCharactersBetween(this GameObject gameObject, float minimumDistance,
            float maximumDistance) {
            var transformArray = Object.FindObjectsOfType<CharacterPresenter>()
                .Select(go => go.transform)
                .Where(
                    t =>
                        Vector3.Distance(t.position, gameObject.transform.position) <= maximumDistance &&
                        Vector3.Distance(t.position, gameObject.transform.position) >= minimumDistance)
                .ToList();
            return transformArray;
        }

        public static List<Transform> GetDefenderCharactersBetween(this GameObject gameObject, float minimumDistance,
            float maximumDistance) {
            var transformArray = Object.FindObjectsOfType<CharacterPresenter>()
                .Where(_ => _.CharacterSide == CharacterPresenter.CharacterSideEnum.Defender)
                .Select(go => go.transform)
                .Where(
                    t =>
                        Vector3.Distance(t.position, gameObject.transform.position) <= maximumDistance &&
                        Vector3.Distance(t.position, gameObject.transform.position) >= minimumDistance)
                .ToList();
            return transformArray;
        }

        public static List<Transform> GetAttackerCharactersBetween(this GameObject gameObject, float minimumDistance,
            float maximumDistance) {
            var transformArray = Object.FindObjectsOfType<CharacterPresenter>()
                .Where(_ => _.CharacterSide == CharacterPresenter.CharacterSideEnum.Attacker)
                .Select(go => go.transform)
                .Where(
                    t =>
                        Vector3.Distance(t.position, gameObject.transform.position) <= maximumDistance &&
                        Vector3.Distance(t.position, gameObject.transform.position) >= minimumDistance)
                .ToList();
            return transformArray;
        }

        public static Animation FindAnimationComponent(this GameObject gameObject) {
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

        public static DynamicGridObstacle FindDynamicGridObstacleComponent(this GameObject gameObject) {
            try
            {
                var gameObjects = gameObject.DescendantsAndSelf().ToList();
                var componentHolder =
                    gameObjects.FirstOrDefault(_ => _.GetComponent<DynamicGridObstacle>() != null);
                if (componentHolder != null)
                    return componentHolder.GetComponent<DynamicGridObstacle>();
                Debug.LogErrorFormat("Object {0} doesn't contains {1} component on itself or any descendant",
                    gameObject.name, typeof(DynamicGridObstacle));
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("Object {0} doesn't contains {1} component on itself or any descendant: {2}",
                    gameObject.name, typeof(DynamicGridObstacle), ex);
                return null;
            }
        }

        public static Animator FindAnimatorComponent(this GameObject gameObject) {
            try
            {
                var gameObjects = gameObject.DescendantsAndSelf().ToList();
                var componentHolder =
                    gameObjects.FirstOrDefault(_ => _.GetComponent<Animator>() != null);
                if (componentHolder != null)
                    return componentHolder.GetComponent<Animator>();
                Debug.LogErrorFormat("Object {0} doesn't contains {1} component on itself or any descendant",
                    gameObject.name, typeof(Animator));
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("Object {0} doesn't contains {1} component on itself or any descendant: {2}",
                    gameObject.name, typeof(Animator), ex);
                return null;
            }
        }

        public static CharacterController FindCharacterControllerComponent(this GameObject gameObject) {
            try
            {
                var gameObjects = gameObject.DescendantsAndSelf().ToList();
                var componentHolder =
                    gameObjects.FirstOrDefault(_ => _.GetComponent<CharacterController>() != null);
                if (componentHolder != null)
                    return componentHolder.GetComponent<CharacterController>();
                Debug.LogErrorFormat("Object {0} doesn't contains {1} component on itself or any descendant",
                    gameObject.name, typeof(CharacterController));
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("Object {0} doesn't contains {1} component on itself or any descendant: {2}",
                    gameObject.name, typeof(CharacterController), ex);
                return null;
            }
        }

        public static NavMeshAgent FindNavMeshAgentComponent(this GameObject gameObject) {
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

        public static NavMeshObstacle FindNavMeshObstacleComponent(this GameObject gameObject) {
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

        public static Slider FindSliderComponent(this GameObject gameObject) {
            try
            {
                var gameObjects = gameObject.DescendantsAndSelf().ToList();
                var componentHolder =
                    gameObjects.FirstOrDefault(_ => _.GetComponent<Slider>() != null);
                if (componentHolder != null)
                    return componentHolder.GetComponent<Slider>();
                Debug.LogErrorFormat("Object {0} doesn't contains {1} component on itself or any descendant",
                    gameObject.name, typeof(Slider));
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("Object {0} doesn't contains {1} component on itself or any descendant: {2}",
                    gameObject.name, typeof(Slider), ex);
                return null;
            }
        }

        public static TextMeshProUGUI FindTextMeshProUguiComponent(this GameObject gameObject) {
            try
            {
                var gameObjects = gameObject.DescendantsAndSelf().ToList();
                var componentHolder =
                    gameObjects.FirstOrDefault(_ => _.GetComponent<TextMeshProUGUI>() != null);
                if (componentHolder != null)
                    return componentHolder.GetComponent<TextMeshProUGUI>();
                Debug.LogErrorFormat("Object {0} doesn't contains {1} component on itself or any descendant",
                    gameObject.name, typeof(TextMeshProUGUI));
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("Object {0} doesn't contains {1} component on itself or any descendant: {2}",
                    gameObject.name, typeof(TextMeshProUGUI), ex);
                return null;
            }
        }
    }
}
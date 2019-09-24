using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(UIObjectAnimator))]
public class UIObjectAnimator_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        UIObjectAnimator script = (UIObjectAnimator)target;

        EditorGUILayout.Separator();

        script.canvasObject = EditorGUILayout.Toggle("Canvas Child Object", script.canvasObject);
        if (!script.canvasObject)
        {
            script.spriteRenderer = (SpriteRenderer)EditorGUILayout.ObjectField("Sprite Renderer Object", script.spriteRenderer, typeof(SpriteRenderer), true);
        }
        script.alsoFade = EditorGUILayout.Toggle("Also Fade", script.alsoFade);

        EditorGUILayout.Separator();

        script.onScreenLerpType = (UIObjectAnimator.LerpType)EditorGUILayout.EnumPopup("Animation Type (On)", script.onScreenLerpType);

        switch (script.onScreenLerpType)
        {
            case UIObjectAnimator.LerpType.scale:
                break;
            case UIObjectAnimator.LerpType.motionUp:
            case UIObjectAnimator.LerpType.motionDown:
            case UIObjectAnimator.LerpType.motionLeft:
            case UIObjectAnimator.LerpType.motionRight:
                if (script.onScreenLerpType == UIObjectAnimator.LerpType.motionUp)
                {
                    EditorGUILayout.HelpBox("Moves on the screen in an upward direction", MessageType.Info);
                }
                else if (script.onScreenLerpType == UIObjectAnimator.LerpType.motionDown)
                {
                    EditorGUILayout.HelpBox("Moves on the screen in a downward direction", MessageType.Info);
                }
                else if (script.onScreenLerpType == UIObjectAnimator.LerpType.motionLeft)
                {
                    EditorGUILayout.HelpBox("Moves on the screen in the left direction", MessageType.Info);
                }
                else if (script.onScreenLerpType == UIObjectAnimator.LerpType.motionRight)
                {
                    EditorGUILayout.HelpBox("Moves on the screen in the right direction", MessageType.Info);
                }
                break;
        }

        EditorGUILayout.Separator();

        script.offScreenLerpType = (UIObjectAnimator.LerpType)EditorGUILayout.EnumPopup("Animation Type (Off)", script.offScreenLerpType);

        switch (script.offScreenLerpType)
        {
            case UIObjectAnimator.LerpType.scale:
                break;
            case UIObjectAnimator.LerpType.motionUp:
            case UIObjectAnimator.LerpType.motionDown:
            case UIObjectAnimator.LerpType.motionLeft:
            case UIObjectAnimator.LerpType.motionRight:
                if (script.offScreenLerpType == UIObjectAnimator.LerpType.motionUp)
                {
                    EditorGUILayout.HelpBox("Moves off the screen in an upward direction", MessageType.Info);
                }
                else if (script.offScreenLerpType == UIObjectAnimator.LerpType.motionDown)
                {
                    EditorGUILayout.HelpBox("Moves off the screen in a downward direction", MessageType.Info);
                }
                else if (script.offScreenLerpType == UIObjectAnimator.LerpType.motionLeft)
                {
                    EditorGUILayout.HelpBox("Moves off the screen in the left direction", MessageType.Info);
                }
                else if (script.offScreenLerpType == UIObjectAnimator.LerpType.motionRight)
                {
                    EditorGUILayout.HelpBox("Moves off the screen in the right direction", MessageType.Info);
                }
                script.movementModifier = EditorGUILayout.DelayedFloatField("Movement Modifier", script.movementModifier);
                if (script.movementModifier == 0)
                {
                    script.movementModifier = 1;
                }
                EditorGUILayout.HelpBox("Movement Modifier is multiplied onto the objects off screen movement, to make it go less distance or more.", MessageType.Info);
                break;
        }

        EditorGUILayout.Separator();
        script.lerpTime = EditorGUILayout.DelayedFloatField("Animation Time", script.lerpTime);

        EditorGUILayout.Separator(); EditorGUILayout.Separator();

        script.enableAnimations = EditorGUILayout.Toggle("Enable Animations", script.enableAnimations);
        if (script.enableAnimations)
        {
            script.mainCamera = (Camera)EditorGUILayout.ObjectField("Main Camera", script.mainCamera, typeof(Camera), true);
            script.mainImage = (Image)EditorGUILayout.ObjectField("Main Image", script.mainImage, typeof(Image), true);
            script.enableSpriteSwapAnimations = EditorGUILayout.Toggle("Enable Sprite Swap Animations", script.enableSpriteSwapAnimations);
            EditorGUILayout.Separator();
            if (script.enableSpriteSwapAnimations)
            {
                script.defaultSprite = (Sprite)EditorGUILayout.ObjectField("Default Sprite", script.defaultSprite, typeof(Sprite), true);
                script.mouseOverSprite = (Sprite)EditorGUILayout.ObjectField("Mouse Over Sprite", script.mouseOverSprite, typeof(Sprite), true);
                script.clickedSprite = (Sprite)EditorGUILayout.ObjectField("Clicked Sprite", script.clickedSprite, typeof(Sprite), true);
                script.inactiveSprite = (Sprite)EditorGUILayout.ObjectField("Inactive Sprite", script.inactiveSprite, typeof(Sprite), true);
            }
            else
            {
                script.interactAnimationType = (UIObjectAnimator.InteractAnimationType)EditorGUILayout.EnumPopup("Interact Animation Type", script.interactAnimationType);
            }
        }

        EditorUtility.SetDirty(target);
    }
}
#endif
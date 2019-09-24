using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIObjectAnimator : MonoBehaviour
{
    public bool animatorActive;
    public bool canvasObject;
    public SpriteRenderer spriteRenderer;
    //public 

    public delegate void OnFinishAnimation();
    OnFinishAnimation onFinishAnimation;

    public LerpType onScreenLerpType;
    public LerpType offScreenLerpType;
    public CustomLerpType currentCustomLerpType;
    public bool alsoFade; // fade in or out along with whatever animation type
    public enum LerpType
    {
        scale,
        motionUp,
        motionDown,
        motionLeft,
        motionRight,
        fade
    }

    public enum CustomLerpType
    { // This can change throughout an objects lifetime
        none,
        offScreenToTargetPosition
    }

    public enum InteractAnimationType
    {
        colorChange
    }

    // Color Change
    Color defaultColor;
    Color mouseOverColor;
    Color clickedColor;
    Color inactiveColor;

    void Awake()
    {
        defaultColor = new Color(1, 1, 1);
        float mouseOverColorVal = MapValue(210, 0, 255, 0, 1);
        mouseOverColor = new Color(mouseOverColorVal, mouseOverColorVal, mouseOverColorVal);
        float clickedColorVal = MapValue(180, 0, 255, 0, 1);
        clickedColor = new Color(clickedColorVal, clickedColorVal, clickedColorVal);
        float inactiveColorVal = MapValue(130, 0, 255, 0, 1);
        inactiveColor = new Color(inactiveColorVal, inactiveColorVal, inactiveColorVal);
        SetActive();
    }

    // Mouse Over and OnClick
    public bool enableAnimations;
    public Camera mainCamera;
    public InteractAnimationType interactAnimationType;
    public bool enableSpriteSwapAnimations;
    public Image mainImage;
    public Sprite defaultSprite;
    public Sprite mouseOverSprite;
    public Sprite clickedSprite;
    public Sprite inactiveSprite;

    // Scale Lerp Type
    Vector3 oldScale;
    Vector3 newScale;

    // Motion Lerp Type
    [Range(0.1f, 10.0f)]
    public float movementModifier = 1; // multiplied onto newPos.x or newPos.y depending on lerp type
    Vector3 oldPos;
    Vector3 newPos;

    // Fade Lerp Type (these also used for motion)
    float oldAlpha;
    float newAlpha;

    // All Lerp Types
    float timeStartedLerping;
    bool isLerping;
    bool comingOnScreen;
    public float lerpTime;

    bool needToResetLerpTime;
    float originalLerpTime;

    public void InitializeAnimator()
    {
        OffScreen(0);
    }

    public void InitializeAnimator(LerpType overrideType)
    {
        OffScreen(0, overrideType);
    }

    [HideInInspector]
    public bool canInteract;
    void Update()
    {
        if (!animatorActive)
        {
            return;
        }
        if (!enableAnimations)
        {
            return;
        }
        if (!canInteract)
        {
            //ClickFinished();
            return;
        }
        if (mainCamera != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            bool animatorHit = false;
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.name.Equals(name))
                {
                    MouseOver();
                    animatorHit = true;
                    if (Input.GetMouseButton(0))
                    {
                        Clicked();
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        ClickFinished();
                    }
                }
            }
            if (!animatorHit)
            {
                MouseLeft();
            }
        }
    }

    public void MouseOver()
    {
        if (enableSpriteSwapAnimations)
        {
            if (mainImage != null)
            {
                if (mouseOverSprite != null)
                {
                    mainImage.sprite = mouseOverSprite;
                }
            }
        }
        else
        {
            if (enableAnimations)
            {
                if (mainImage != null)
                {
                    switch (interactAnimationType)
                    {
                        case InteractAnimationType.colorChange:
                            mainImage.color = mouseOverColor;
                            break;
                        default:
                            mainImage.color = mouseOverColor;
                            break;
                    }
                }
            }
        }
    }

    public void MouseLeft()
    {
        if (enableSpriteSwapAnimations)
        {
            if (mainImage != null)
            {
                if (defaultSprite != null)
                {
                    mainImage.sprite = defaultSprite;
                }
            }
        }
        else
        {
            if (enableAnimations)
            {
                if (mainImage != null)
                {
                    switch (interactAnimationType)
                    {
                        case InteractAnimationType.colorChange:
                            mainImage.color = defaultColor;
                            break;
                    }
                }
            }
        }
    }

    public void Clicked()
    {
        if (enableSpriteSwapAnimations)
        {
            if (mainImage != null)
            {
                if (clickedSprite != null)
                {
                    mainImage.sprite = clickedSprite;
                }
            }
        }
        else
        {
            if (enableAnimations)
            {
                if (mainImage != null)
                {
                    switch (interactAnimationType)
                    {
                        case InteractAnimationType.colorChange:
                            mainImage.color = clickedColor;
                            break;
                    }
                }
            }
        }
    }

    void ClickFinished()
    {
        if (enableSpriteSwapAnimations)
        {
            if (mainImage != null)
            {
                if (defaultSprite != null)
                {
                    mainImage.sprite = defaultSprite;
                }
            }
        }
        else
        {
            if (enableAnimations)
            {
                if (mainImage != null)
                {
                    switch (interactAnimationType)
                    {
                        case InteractAnimationType.colorChange:
                            mainImage.color = defaultColor;
                            break;
                    }
                }
            }
        }
    }

    public void SetActive()
    {
        animatorActive = true;
        if (enableSpriteSwapAnimations)
        {
            if (mainImage != null)
            {
                if (defaultSprite != null)
                {
                    mainImage.sprite = defaultSprite;
                }
            }
        }
        else
        {
            if (enableAnimations)
            {
                if (mainImage != null)
                {
                    switch (interactAnimationType)
                    {
                        case InteractAnimationType.colorChange:
                            mainImage.color = defaultColor;
                            break;
                    }
                }
            }
        }
    }

    public void SetInactive()
    {
        animatorActive = false;
        if (enableSpriteSwapAnimations)
        {
            if (mainImage != null)
            {
                if (inactiveSprite != null)
                {
                    mainImage.sprite = inactiveSprite;
                }
            }
        }
        else
        {
            if (enableAnimations)
            {
                if (mainImage != null)
                {
                    switch (interactAnimationType)
                    {
                        case InteractAnimationType.colorChange:
                            mainImage.color = inactiveColor;
                            break;
                    }
                }
            }
        }
    }

    public void OnScreen(float updatedLerpTime)
    {
        originalLerpTime = lerpTime;
        lerpTime = updatedLerpTime;
        needToResetLerpTime = true;
        OnScreen();
    }

    public void OnScreen()
    {
        OffScreen(0.0f);
        gameObject.SetActive(true);
        Color originalColor = Color.white;
        try
        {
            originalColor = spriteRenderer.color;
        }
        catch (System.Exception ex)
        {
            try
            {
                originalColor = GetComponent<Image>().color;
            }
            catch (System.Exception ex2)
            {
                originalColor = GetComponent<Text>().color;
            }
        }
        switch (onScreenLerpType)
        {
            case LerpType.scale:
                oldScale = transform.localScale;
                newScale = Vector3.one;
                break;
            case LerpType.motionUp:
            case LerpType.motionDown:
            case LerpType.motionLeft:
            case LerpType.motionRight:
                if (spriteRenderer != null)
                {
                    oldPos = transform.localPosition;
                    newPos = Vector3.zero;
                }
                else
                {
                    oldPos = GetComponent<RectTransform>().anchoredPosition;
                    newPos = Vector2.zero;
                }
                oldAlpha = originalColor.a;
                newAlpha = 1;
                break;
            case LerpType.fade:
                oldAlpha = originalColor.a;
                newAlpha = 1;
                break;
        }
        isLerping = true;
        timeStartedLerping = Time.time;
        comingOnScreen = true;
    }

    public void OffScreen(float updatedLerpTime)
    {
        SetActive();
        originalLerpTime = lerpTime;
        lerpTime = updatedLerpTime;
        needToResetLerpTime = true;
        OffScreen();
    }

    public void OffScreen(float updatedLerpTime, LerpType overrideType)
    {
        SetActive();
        originalLerpTime = lerpTime;
        lerpTime = updatedLerpTime;
        needToResetLerpTime = true;
        OffScreenOverride(overrideType);
    }

    public void OffScreenOverride(LerpType overrideType)
    {
        Color originalColor = Color.white;
        try
        {
            originalColor = spriteRenderer.color;
        }
        catch (System.Exception ex)
        {
            try
            {
                originalColor = GetComponent<Image>().color;
            }
            catch (System.Exception ex2)
            {
                originalColor = GetComponent<Text>().color;
            }
        }
        switch (overrideType)
        {
            case LerpType.scale:
                oldScale = transform.localScale;
                newScale = Vector3.zero;
                break;
            case LerpType.motionUp:
                if (spriteRenderer != null)
                {
                    float height = spriteRenderer.sprite.rect.height;
                    oldPos = transform.localPosition;
                    newPos = oldPos + new Vector3(0, height * movementModifier, 0);
                }
                else
                {
                    RectTransform rectTransform = GetComponent<RectTransform>();
                    float height = rectTransform.rect.height;
                    oldPos = rectTransform.anchoredPosition;
                    newPos = new Vector2(0, height * movementModifier);
                }
                oldAlpha = originalColor.a;
                newAlpha = 0;
                break;
            case LerpType.motionDown:
                if (spriteRenderer != null)
                {
                    float height = spriteRenderer.sprite.rect.height;
                    oldPos = transform.localPosition;
                    newPos = oldPos + new Vector3(0, -height * movementModifier, 0);
                }
                else
                {
                    RectTransform rectTransform_ = GetComponent<RectTransform>();
                    float height_ = rectTransform_.rect.height;
                    oldPos = rectTransform_.anchoredPosition;
                    newPos = new Vector2(0, -height_ * movementModifier);
                }
                oldAlpha = originalColor.a;
                newAlpha = 0;
                break;
            case LerpType.motionLeft:
                if (spriteRenderer != null)
                {
                    float width = spriteRenderer.sprite.rect.height;
                    oldPos = transform.localPosition;
                    newPos = oldPos + new Vector3(-width * movementModifier, 0);
                }
                else
                {
                    RectTransform rectTransform_ = GetComponent<RectTransform>();
                    float width_ = rectTransform_.rect.height;
                    oldPos = rectTransform_.anchoredPosition;
                    newPos = new Vector2(-width_ * movementModifier, 0);
                }
                oldAlpha = originalColor.a;
                newAlpha = 0;
                break;
            case LerpType.motionRight:
                if (spriteRenderer != null)
                {
                    float width = spriteRenderer.sprite.rect.height;
                    oldPos = transform.localPosition;
                    newPos = oldPos + new Vector3(width * movementModifier, 0);
                }
                else
                {
                    RectTransform rectTransform_ = GetComponent<RectTransform>();
                    float width_ = rectTransform_.rect.height;
                    oldPos = rectTransform_.anchoredPosition;
                    newPos = new Vector2(width_ * movementModifier, 0);
                }
                oldAlpha = originalColor.a;
                newAlpha = 0;
                break;
            case LerpType.fade:
                oldAlpha = originalColor.a;
                newAlpha = 0;
                break;
        }
        isLerping = true;
        timeStartedLerping = Time.time;
        comingOnScreen = false;
    }

    public void OffScreen()
    {
        Color originalColor = Color.white;
        try
        {
            originalColor = spriteRenderer.color;
        }
        catch (System.Exception ex)
        {
            try
            {
                originalColor = GetComponent<Image>().color;
            }
            catch (System.Exception ex2)
            {
                originalColor = GetComponent<Text>().color;
            }
        }
        switch (offScreenLerpType)
        {
            case LerpType.scale:
                oldScale = transform.localScale;
                newScale = Vector3.zero;
                break;
            case LerpType.motionUp:
                if (spriteRenderer != null)
                {
                    float height = spriteRenderer.sprite.rect.height;
                    oldPos = transform.localPosition;
                    newPos = oldPos + new Vector3(0, height * movementModifier, 0);
                }
                else
                {
                    RectTransform rectTransform = GetComponent<RectTransform>();
                    float height = rectTransform.rect.height;
                    oldPos = rectTransform.anchoredPosition;
                    newPos = new Vector2(0, height * movementModifier);
                }
                oldAlpha = originalColor.a;
                newAlpha = 0;
                break;
            case LerpType.motionDown:
                if (spriteRenderer != null)
                {
                    float height = spriteRenderer.sprite.rect.height;
                    oldPos = transform.localPosition;
                    newPos = oldPos + new Vector3(0, -height * movementModifier, 0);
                }
                else
                {
                    RectTransform rectTransform_ = GetComponent<RectTransform>();
                    float height_ = rectTransform_.rect.height;
                    oldPos = rectTransform_.anchoredPosition;
                    newPos = new Vector2(0, -height_ * movementModifier);
                }
                oldAlpha = originalColor.a;
                newAlpha = 0;
                break;
            case LerpType.motionLeft:
                if (spriteRenderer != null)
                {
                    float width = spriteRenderer.sprite.rect.height;
                    oldPos = transform.localPosition;
                    newPos = oldPos + new Vector3(-width * movementModifier, 0);
                }
                else
                {
                    RectTransform rectTransform_ = GetComponent<RectTransform>();
                    float width_ = rectTransform_.rect.height;
                    oldPos = rectTransform_.anchoredPosition;
                    newPos = new Vector2(-width_ * movementModifier, 0);
                }
                oldAlpha = originalColor.a;
                newAlpha = 0;
                break;
            case LerpType.motionRight:
                if (spriteRenderer != null)
                {
                    float width = spriteRenderer.sprite.rect.height;
                    oldPos = transform.localPosition;
                    newPos = oldPos + new Vector3(width * movementModifier, 0);
                }
                else
                {
                    RectTransform rectTransform_ = GetComponent<RectTransform>();
                    float width_ = rectTransform_.rect.height;
                    oldPos = rectTransform_.anchoredPosition;
                    newPos = new Vector2(width_ * movementModifier, 0);
                }
                oldAlpha = originalColor.a;
                newAlpha = 0;
                break;
            case LerpType.fade:
                oldAlpha = originalColor.a;
                newAlpha = 0;
                break;
        }
        isLerping = true;
        timeStartedLerping = Time.time;
        comingOnScreen = false;
    }

    public void OffScreenToTargetPos(Vector3 targetPos, float customLerpTime)
    { // Will move an object towards a target position
        currentCustomLerpType = CustomLerpType.offScreenToTargetPosition;
        RectTransform rectTransform_ = GetComponent<RectTransform>();
        oldPos = transform.localPosition;
        newPos = targetPos;
        oldAlpha = 1;
        newAlpha = 0;
        originalLerpTime = lerpTime;
        lerpTime = customLerpTime;
        needToResetLerpTime = true;
        isLerping = true;
        timeStartedLerping = Time.time;
        comingOnScreen = false;
    }

    void FixedUpdate()
    {
        if (isLerping)
        {
            float timeSinceStart = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStart / ((lerpTime == 0) ? originalLerpTime : lerpTime);

            if (currentCustomLerpType == CustomLerpType.none)
            {
                switch ((comingOnScreen) ? onScreenLerpType : offScreenLerpType)
                {
                    case LerpType.scale:
                        transform.localScale = Vector3.Lerp(oldScale, newScale, percentageComplete);
                        break;
                    case LerpType.motionUp:
                    case LerpType.motionDown:
                    case LerpType.motionLeft:
                    case LerpType.motionRight:
                        if (spriteRenderer != null)
                        {
                            transform.localPosition = Vector3.Lerp(oldPos, newPos, percentageComplete);

                            if (alsoFade)
                            {
                                Color originalColor = spriteRenderer.color;
                                Vector2 alphaLerp = Vector2.Lerp(new Vector2(oldAlpha, 0), new Vector2(newAlpha, 0), percentageComplete);
                                Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, alphaLerp.x);
                                spriteRenderer.color = newColor;
                            }
                        }
                        else
                        {
                            RectTransform rectTransform = GetComponent<RectTransform>();
                            rectTransform.anchoredPosition = Vector2.Lerp(oldPos, newPos, percentageComplete);

                            if (alsoFade)
                            {
                                // Gather images and text to be recolored
                                Image[] childImages = GetComponentsInChildren<Image>();
                                List<Image> imageList = new List<Image>();
                                imageList.Add(GetComponent<Image>());
                                foreach (Image img in childImages)
                                {
                                    imageList.Add(img);
                                }

                                // Recolor gathered images
                                Color originalColor = GetComponent<Image>().color;
                                Vector2 alphaLerp = Vector2.Lerp(new Vector2(oldAlpha, 0), new Vector2(newAlpha, 0), percentageComplete);
                                Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, alphaLerp.x);
                                foreach (Image img in imageList)
                                {
                                    img.color = newColor;
                                }
                            }
                        }
                        break;
                    case LerpType.fade:
                        if (spriteRenderer != null)
                        {
                            Color originalColor = spriteRenderer.color;
                            Vector2 alphaLerp = Vector2.Lerp(new Vector2(oldAlpha, 0), new Vector2(newAlpha, 0), percentageComplete);
                            Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, alphaLerp.x);
                            spriteRenderer.color = newColor;
                        }
                        else
                        {

                            Image img = GetComponent<Image>();
                            Color originalColor_ = img.color;
                            Vector2 alphaLerp_ = Vector2.Lerp(new Vector2(oldAlpha, 0), new Vector2(newAlpha, 0), percentageComplete);
                            Color newColor_ = new Color(originalColor_.r, originalColor_.g, originalColor_.b, alphaLerp_.x);
                            img.color = newColor_;

                            /*
                            // Gather images and text to be recolored
                            Image[] childImages_ = GetComponentsInChildren<Image>();
                            List<Image> imageList_ = new List<Image>();
                            imageList_.Add(GetComponent<Image>());
                            foreach (Image img in childImages_)
                            {
                                imageList_.Add(img);
                            }

                            // Recolor gathered images
                            Color originalColor_ = GetComponent<Image>().color;
                            Vector2 alphaLerp_ = Vector2.Lerp(new Vector2(oldAlpha, 0), new Vector2(newAlpha, 0), percentageComplete);
                            Color newColor_ = new Color(originalColor_.r, originalColor_.g, originalColor_.b, alphaLerp_.x);

                            foreach (Image img in imageList_)
                            {
                                print(img.name);
                                img.color = newColor_;
                            }*/
                        }
                        break;
                }
            }
            else if (currentCustomLerpType == CustomLerpType.offScreenToTargetPosition)
            {
                if (spriteRenderer != null)
                {
                    transform.localPosition = Vector3.Lerp(oldPos, newPos, percentageComplete);
                    if (alsoFade)
                    {
                        Color originalColor = spriteRenderer.color;
                        Vector2 alphaLerp = Vector2.Lerp(new Vector2(oldAlpha, 0), new Vector2(newAlpha, 0), percentageComplete);
                        Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, alphaLerp.x);
                        spriteRenderer.color = newColor;
                    }
                }
                else
                {
                    transform.localPosition = Vector3.Lerp(oldPos, newPos, percentageComplete);
                    //RectTransform rectTransform = GetComponent<RectTransform>();
                    //rectTransform.anchoredPosition = Vector2.Lerp(oldPos, newPos, percentageComplete);
                    if (alsoFade)
                    {
                        // Gather images and text to be recolored
                        Image[] childImages = GetComponentsInChildren<Image>();
                        List<Image> imageList = new List<Image>();
                        imageList.Add(GetComponent<Image>());
                        foreach (Image img in childImages)
                        {
                            imageList.Add(img);
                        }

                        // Recolor gathered images
                        Color originalColor = GetComponent<Image>().color;
                        Vector2 alphaLerp = Vector2.Lerp(new Vector2(oldAlpha, 0), new Vector2(newAlpha, 0), percentageComplete);
                        Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, alphaLerp.x);
                        foreach (Image img in imageList)
                        {
                            img.color = newColor;
                        }
                    }
                }
            }

            if (percentageComplete >= 1f)
            {
                if (!comingOnScreen)
                {
                    gameObject.SetActive(false);
                }
                if (needToResetLerpTime)
                {
                    lerpTime = originalLerpTime;
                }
                isLerping = false;
                if (onFinishAnimation != null)
                {
                    onFinishAnimation();
                }
            }
        }
    }

    public void AddFinishDelegate(OnFinishAnimation newOnFinish)
    {
        onFinishAnimation -= newOnFinish;
        onFinishAnimation += newOnFinish;
    }

    public float MapValue(float currentValue, float x, float y, float newX, float newY)
    {
        // Maps value from x - y  to  0 - 1.
        return (newX + ((currentValue - x) * (newY - newX) / (y - x)));
    }
}//795
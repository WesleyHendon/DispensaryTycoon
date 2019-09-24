using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteManager : MonoBehaviour
{
    // Screen borders
    public static Sprite screenborder_Green;
    public static Sprite screenborder_Red;
    public static Sprite screenborder_Yellow;
    public Sprite screenborder_Green_;
    public Sprite screenborder_Red_;
    public Sprite screenborder_Yellow_;

    // For notifications
    public static Sprite notificationIcon_Error;
    public Sprite notificationIcon_Error_;

    // For Dropdowns
    public static Sprite defaultDropdownSprite;
    public static Sprite selectedDropdownSprite;
    public Sprite defaultDropdownSprite_;
    public Sprite selectedDropdownSprite_;
    
    // For Vendor Panels tabs
    public static Sprite selectedTabSprite;
    public static Sprite unselectedTabSprite;
    public Sprite selectedTabSprite_;
    public Sprite unselectedTabSprite_;

    // Vendor/orders
    public static Sprite orderDisplayPanel_DisplayingPresetsSprite;
    public static Sprite orderDisplayPanel_DisplayingOrderSprite;
    public Sprite orderDisplayPanel_DisplayingPresetsSprite_;
    public Sprite orderDisplayPanel_DisplayingOrderSprite_;

    // For Inventory UI
    public static Sprite visibleInListSprite;
    public static Sprite notVisibleInListSprite;
    public static Sprite mixedVisibilitySprite;
    public Sprite visibleInListSprite_;
    public Sprite notVisibleInListSprite_;
    public Sprite mixedVisibilitySprite_;

    // Game speed
    public static Sprite regularSpeedButtonSprite;
    public static Sprite twoTimesSpeedButtonSprite;
    public static Sprite threeTimesSpeedButtonSprite;
    public static Sprite fiveTimesSpeedButtonSprite;
    public Sprite regularSpeedButtonSprite_;
    public Sprite twoTimesSpeedButtonSprite_;
    public Sprite threeTimesSpeedButtonSprite_;
    public Sprite fiveTimesSpeedButtonSprite_;

    // Job Icons
    public static Sprite storeBudtenderIcon;
    public static Sprite smokeBudtenderIcon;
    public static Sprite cashierIcon;
    public static Sprite securityIcon;
    public static Sprite janitorIcon;
    public Sprite storeBudtenderIcon_;
    public Sprite smokeBudtenderIcon_;
    public Sprite cashierIcon_;
    public Sprite securityIcon_;
    public Sprite janitorIcon_;

    // Event Scheduler Sprites
    public static Sprite deliveryEventSprite;
    public static Sprite smokeLoungeEventSprite;
    public static Sprite glassShopEventSprite;
    public static Sprite growroomEventSprite;
    public static Sprite selectedDeliveryEventSprite;
    public static Sprite selectedSmokeLoungeEventSprite;
    public static Sprite selectedGlassShopEventSprite;
    public static Sprite selectedGrowroomEventSprite;
    public static Sprite lockedEventSprite;
    public Sprite deliveryEventSprite_;
    public Sprite smokeLoungeEventSprite_;
    public Sprite glassShopEventSprite_;
    public Sprite growroomEventSprite_;
    public Sprite selectedDeliveryEventSprite_;
    public Sprite selectedSmokeLoungeEventSprite_;
    public Sprite selectedGlassShopEventSprite_;
    public Sprite selectedGrowroomEventSprite_;
    public Sprite lockedEventSprite_;

    // Visual Indicator Sprites
    public static Sprite AIerrorIndicator;
    public static Sprite AIselectedIndicator;
    public Sprite AIerrorIndicator_;
    public Sprite AIselectedIndicator_;

    // For keybindings input fields
    public static Sprite regularFieldSprite;
    public static Sprite activatedFieldSprite;
    public static Sprite errorFieldSprite;
    public Sprite regularFieldSprite_;
    public Sprite activatedFieldSprite_;
    public Sprite errorFieldSprite_;

    void Start()
    { // Assign all static variables when the instance is created.
        if (screenborder_Green_ != null)
        {
            screenborder_Green = screenborder_Green_;
        }
        if (screenborder_Red_ != null)
        {
            screenborder_Red = screenborder_Red_;
        }
        if (screenborder_Yellow_ != null)
        {
            screenborder_Yellow = screenborder_Yellow_;
        }
        if (notificationIcon_Error_ != null)
        {
            notificationIcon_Error = notificationIcon_Error_;
        }
        if (defaultDropdownSprite_ != null)
        {
            defaultDropdownSprite = defaultDropdownSprite_;
        }
        if (selectedDropdownSprite_ != null)
        {
            selectedDropdownSprite = selectedDropdownSprite_;
        }
        if (selectedTabSprite_ != null)
        {
            selectedTabSprite = selectedTabSprite_;
        }
        if (unselectedTabSprite_ != null)
        {
            unselectedTabSprite = unselectedTabSprite_;
        }
        if (orderDisplayPanel_DisplayingPresetsSprite_ != null)
        {
            orderDisplayPanel_DisplayingPresetsSprite = orderDisplayPanel_DisplayingPresetsSprite_;
        }
        if (orderDisplayPanel_DisplayingOrderSprite_ != null)
        {
            orderDisplayPanel_DisplayingOrderSprite = orderDisplayPanel_DisplayingOrderSprite_;
        }
        if (visibleInListSprite_ != null)
        {
            visibleInListSprite = visibleInListSprite_;
        }
        if (notVisibleInListSprite_ != null)
        {
            notVisibleInListSprite = notVisibleInListSprite_;
        }
        if (mixedVisibilitySprite_ != null)
        {
            mixedVisibilitySprite = mixedVisibilitySprite_;
        }
        if (regularSpeedButtonSprite_ != null)
        {
            regularSpeedButtonSprite = regularSpeedButtonSprite_;
        }
        if (twoTimesSpeedButtonSprite_ != null)
        {
            twoTimesSpeedButtonSprite = twoTimesSpeedButtonSprite_;
        }
        if (threeTimesSpeedButtonSprite_ != null)
        {
            threeTimesSpeedButtonSprite = threeTimesSpeedButtonSprite_;
        }
        if (fiveTimesSpeedButtonSprite_ != null)
        {
            fiveTimesSpeedButtonSprite = fiveTimesSpeedButtonSprite_;
        }

        // Job Icon Sprites
        if (storeBudtenderIcon_ != null)
        {
            storeBudtenderIcon = storeBudtenderIcon_;
        }
        if (smokeBudtenderIcon_ != null)
        {
            smokeBudtenderIcon = smokeBudtenderIcon_;
        }
        if (cashierIcon_ != null)
        {
            cashierIcon = cashierIcon_;
        }
        if (securityIcon_ != null)
        {
            securityIcon = securityIcon_;
        }
        if (janitorIcon_ != null)
        {
            janitorIcon = janitorIcon_;
        }

        // Event Scheduler Sprites
        if (deliveryEventSprite_ != null)
        {
            deliveryEventSprite = deliveryEventSprite_;
        }
        if (smokeLoungeEventSprite_ != null)
        {
            smokeLoungeEventSprite = smokeLoungeEventSprite_;
        }
        if (glassShopEventSprite_ != null)
        {
            glassShopEventSprite = glassShopEventSprite_;
        }
        if (growroomEventSprite_ != null)
        {
            growroomEventSprite = growroomEventSprite_;
        }
        if (selectedDeliveryEventSprite_ != null)
        {
            selectedDeliveryEventSprite = selectedDeliveryEventSprite_;
        }
        if (selectedSmokeLoungeEventSprite_ != null)
        {
            selectedSmokeLoungeEventSprite = selectedSmokeLoungeEventSprite_;
        }
        if (selectedGlassShopEventSprite_ != null)
        {
            selectedGlassShopEventSprite = selectedGlassShopEventSprite_;
        }
        if (selectedGrowroomEventSprite_ != null)
        {
            selectedGrowroomEventSprite = selectedGrowroomEventSprite_;
        }
        if (lockedEventSprite_ != null)
        {
            lockedEventSprite = lockedEventSprite_;
        }

        // Indicator Sprites
        if (AIerrorIndicator_ != null)
        {
            AIerrorIndicator = AIerrorIndicator_;
        }
        if (AIselectedIndicator_ != null)
        {
            AIselectedIndicator = AIselectedIndicator_;
        }
        if (regularFieldSprite_ != null)
        {
            regularFieldSprite = regularFieldSprite_;
        }
        if (activatedFieldSprite_ != null)
        {
            activatedFieldSprite = activatedFieldSprite_;
        }
        if (errorFieldSprite_ != null)
        {
            errorFieldSprite = errorFieldSprite_;
        }
    }
}

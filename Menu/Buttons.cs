using Axiom.Classes;
using Axiom.Mods;
using Axiom.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using static Axiom.Menu.Main;
using static Axiom.Settings;
using Axiom.Extensions;

namespace Axiom.Menu
{
    public class Buttons
    {
        public static Category[] categories = new Category[]
        {
            new Category { // Main Mods [0]
                Name = "Home",
                Icon = "home",
                Buttons =
                {
                    new ButtonInfo { buttonText = "Welcome to Axiom, User", label = true },
                    new ButtonInfo {buttonText = "Join The Discord :P", method = () => Process.Start("https://discord.gg/XCWc2ezstp"), isTogglable = false, toolTip = "Brings you to the Axiom Discord Server."}
                }
            },
            new Category {
                Name = "Settings",
                Icon = "settings",
                Subcategories = {
                    new Category
                    {
                        Name = "Menu Settings",
                        Buttons =
                        {
                            new ButtonInfo { buttonText = "Right Hand", enableMethod =() => rightHanded = true, disableMethod =() => rightHanded = false, toolTip = "Puts the menu on your right hand."},
                            new ButtonInfo { buttonText = "Notifications", enableMethod =() => disableNotifications = false, disableMethod =() => disableNotifications = true, enabled = !disableNotifications, toolTip = "Toggles the notifications."},
                            new ButtonInfo { buttonText = "FPS Counter", enableMethod =() => fpsCounter = true, disableMethod =() => fpsCounter = false, enabled = fpsCounter, toolTip = "Toggles the FPS counter."},
                            new ButtonInfo { buttonText = "Disconnect Button", enableMethod =() => disconnectButton = true, disableMethod =() => disconnectButton = false, enabled = disconnectButton, toolTip = "Toggles the disconnect button."},
                        }
                    },
                    new Category
                    {
                        Name = "Movement Settings",
                        Buttons =
                        {
                            new ButtonInfo { buttonText = "Change Fly Speed", overlapText = "Change Fly Speed [Normal]", enableMethod =() => Settings.ChangeFlySpeed(true), disableMethod = () => Settings.ChangeFlySpeed(false), incremental = true, isTogglable = false, toolTip = "Changes the speed of the fly mod."},
                        }
                    }
                }
            },
            new Category
            {
                Name = "Room",
                Icon = "room",
                Buttons =
                {
                    new ButtonInfo { buttonText = "Disconnect", method =() => NetworkSystem.Instance.ReturnToSinglePlayer(), isTogglable = false, toolTip = "Disconnects you from the room."},
                }
            },
            new Category
            {
                Name = "Movement",
                Icon = "movement",
                Buttons =
                {
                    new ButtonInfo { buttonText = "Platforms", method =() => Movement.Platforms(), toolTip = "Spawns platforms on your hands when pressing grip."},
                    new ButtonInfo { buttonText = "Fly", method =() => Movement.Fly(), toolTip = "Sends you forward when holding A."},
                    new ButtonInfo { buttonText = "Teleport Gun", method =() => Movement.TeleportGun(), disableMethod = Other.GunLibfix, toolTip = "Teleports you to wherever your pointer is when pressing trigger."},
                    new ButtonInfo { buttonText = "WASD fly", method =() => Movement.WASDFly(), toolTip = "Allows you to fly with WASD"}
                }
            },
            new Category
            {
                Name = "Safety",
                Buttons =
                {
                    
                }
            },
            new Category
            {
                Buttons =
                {
                    new ButtonInfo { buttonText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", isTogglable = false },
                    new ButtonInfo { buttonText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", isTogglable = false },
                    new ButtonInfo { buttonText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", isTogglable = false },
                    new ButtonInfo { buttonText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", isTogglable = false },
                    new ButtonInfo { buttonText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", isTogglable = false },
                    new ButtonInfo { buttonText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", isTogglable = false },
                    new ButtonInfo { buttonText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", isTogglable = false },
                    new ButtonInfo { buttonText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", isTogglable = false },
                    new ButtonInfo { buttonText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", isTogglable = false },
                    new ButtonInfo { buttonText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", isTogglable = false },
                    new ButtonInfo { buttonText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", isTogglable = false },
                    new ButtonInfo { buttonText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", isTogglable = false },
                    new ButtonInfo { buttonText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", isTogglable = false },
                    new ButtonInfo { buttonText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", isTogglable = false },
                    new ButtonInfo { buttonText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", isTogglable = false },
                    new ButtonInfo { buttonText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", isTogglable = false },
                    new ButtonInfo { buttonText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", isTogglable = false },
                    new ButtonInfo { buttonText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", isTogglable = false },
                    new ButtonInfo { buttonText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", isTogglable = false },
                    new ButtonInfo { buttonText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", isTogglable = false },
                    new ButtonInfo { buttonText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", isTogglable = false },
                }
            }

        };

        public static int _currentCategoryIndex;
        public static event Action OnCategoryChanged;

        public static int CurrentCategoryIndex
        {
            get => _currentCategoryIndex;
            set
            {
                _currentCategoryIndex = value;
                pageNumber = 0;
                pageOffset = 0;

                OnCategoryChanged?.Invoke();
            }
        }

        //[Obsolete("Pretty sure this dont work no more")]
        //public static string CurrentCategoryName
        //{
        //    get => Buttons.categoryNames[CurrentCategoryIndex];
        //    set =>
        //        CurrentCategoryIndex = Buttons.GetCategory(value);
        //}

        private static readonly Dictionary<string, (int Category, int Index)> cacheGetIndex = new Dictionary<string, (int Category, int Index)>(); // Looping through 800 elements is not a light task :/
        public static ButtonInfo GetIndex(string buttonText)
        {
            if (string.IsNullOrEmpty(buttonText))
                return null;

            foreach (Category category in categories)
            {
                ButtonInfo button = FindButtonRecursive(category, buttonText);

                if (button != null)
                    return button;
            }

            return null;
        }

        private static ButtonInfo FindButtonRecursive(Category category, string buttonText)
        {
            ButtonInfo button = category.GetButton(buttonText);

            if (button != null)
                return button;

            foreach (Category subcategory in category.Subcategories)
            {
                button = FindButtonRecursive(subcategory, buttonText);

                if (button != null)
                    return button;
            }

            return null;
        }

        //public static int GetCategory(string categoryName) =>
        //        categoryNames.ToList().IndexOf(categoryName);
    }
}

public class UpdateButtonText : MonoBehaviour
{
    public ButtonInfo button;
    public int buttonIndex;
    public float offset;

    private TextMeshPro tmp;
    private UIColorChanger colorChanger;
    private bool initialized;
    private string lastRendered;

    public void Init(ButtonInfo b, int idx, float off)
    {
        button = b;
        buttonIndex = idx;
        offset = off;

        EnsureReferences();
        ApplyLayout();
        initialized = true;
    }

    private void EnsureReferences()
    {
        if (tmp == null) tmp = GetComponent<TextMeshPro>();
        if (colorChanger == null)
            colorChanger = GetComponent<UIColorChanger>() ?? gameObject.AddComponent<UIColorChanger>();
    }

    private void ApplyLayout()
    {
        RectTransform textTransform = tmp.rectTransform;
        textTransform.localPosition = Vector3.zero;
        textTransform.sizeDelta = new Vector2(button != null && button.incremental ? .18f : .2f, .03f * (0.6f / 0.1f));
        //if (NoAutoSizeText) textTransform.sizeDelta = new Vector2(9f, 0.015f);
        //if (hideTextOnCamera) textTransform.gameObject.layer = 19;
        textTransform.localPosition = new Vector3(.064f, 0, .111f - offset / 2.6f);
        textTransform.localRotation = Quaternion.Euler(180f, 90f, 90f);

        tmp.font = activeFont;
        //tmp.spriteAsset = ButtonSpriteSheet;
        tmp.richText = true;
        tmp.fontSize = 1;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = activeFontStyle;
        tmp.enableAutoSizing = true;
        tmp.fontSizeMin = 0;
    }

    public void UpdateText()
    {
        if (!initialized || button == null) return;
        EnsureReferences();
        ApplyLayout();

        string targetButtonText = ButtonText();
        lastRendered = targetButtonText;
        tmp.SafeSetText(targetButtonText);
    }

    private void LateUpdate()
    {
        if (!initialized || button == null || tmp == null) return;
        EnsureReferences();
    }

    private string ButtonText()
    {
        string targetButtonText = button.overlapText ?? button.buttonText;
        return targetButtonText;
    }
}
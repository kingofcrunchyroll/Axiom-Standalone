using Axiom.Classes;
using Axiom.Managers;
using Axiom.Notifications;
using BepInEx;
using GorillaLocomotion;
using HarmonyLib;
using Oculus.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;
using static Axiom.Menu.Buttons;
using static Axiom.Settings;

/*
 * Hello, current and future developers!
 * This is ii's Stupid Template, a base mod menu template for Gorilla Tag.
 * 
 * Comments are placed around the code showing you how certain classes work, such as the settings, buttons, and notifications.
 * 
 * If you need help with the template, you may join my Discord server: https://discord.gg/iidk
 * It's full of talented developers that can show you the way and how things work.
 * 
 * If you want to support my, check out my Patreon: https://patreon.com/iiDk
 * Any support is appreciated, and it helps me make more free content for you all!
 * 
 * Thank you, and enjoy the template!
 */

namespace Axiom.Menu
{
    [HarmonyPatch(typeof(GTPlayer), "LateUpdate")]
    public class Main : MonoBehaviour
    {
        // Constant
        public static void Prefix()
        {
            // Initialize Menu
                try
                {
                    bool toOpen = (!rightHanded && ControllerInputPoller.instance.leftControllerSecondaryButton) || (rightHanded && ControllerInputPoller.instance.rightControllerSecondaryButton);
                    bool keyboardOpen = UnityInput.Current.GetKey(keyboardButton);

                if (currentCategory == null)
                    currentCategory = categories[0];

                if (menu == null)
                    {
                        if (toOpen || keyboardOpen)
                        {
                            CreateMenu();
                            RecenterMenu(rightHanded, keyboardOpen);
                            if (reference == null)
                                CreateReference(rightHanded);
                        }
                    }
                    else
                    {
                        if (toOpen || keyboardOpen)
                            RecenterMenu(rightHanded, keyboardOpen);
                        else
                        {
                            GameObject.Find("Shoulder Camera").transform.Find("CM vcam1").gameObject.SetActive(true);

                            Rigidbody comp = menu.AddComponent(typeof(Rigidbody)) as Rigidbody;
                            comp.linearVelocity = (rightHanded ? GTPlayer.Instance.RightHand.velocityTracker : GTPlayer.Instance.LeftHand.velocityTracker).GetAverageVelocity(true, 0);

                            Destroy(menu, 2f);
                            menu = null;

                            Destroy(reference);
                            reference = null;
                        }
                    }
                }
                catch (Exception exc)
                {
                    Debug.LogError(string.Format("{0} // Error initializing at {1}: {2}", PluginInfo.Name, exc.StackTrace, exc.Message));
                }

            // Constant
                try
                {
                    // Pre-Execution
                        if (fpsObject != null)
                            fpsObject.text = "FPS: " + Mathf.Ceil(1f / Time.unscaledDeltaTime).ToString();

                    // Execute Enabled Mods
                    foreach (Category category in categories)
                    {
                        foreach (ButtonInfo button in GetAllButtons(category))
                        {
                            if (!button.enabled || button.method == null)
                                continue;

                            try
                            {
                                button.method.Invoke();
                            }
                            catch (Exception exc)
                            {
                                Debug.LogError(
                                    string.Format(
                                        "{0} // Error with mod {1} at {2}: {3}",
                                        PluginInfo.Name,
                                        button.buttonText,
                                        exc.StackTrace,
                                        exc.Message
                                    )
                                );
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    Debug.LogError(string.Format("{0} // Error with executing mods at {1}: {2}", PluginInfo.Name, exc.StackTrace, exc.Message));
                }
            }

        // Functions
        public static void CreateMenu()
        {
            // Menu Holder
                menu = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(menu.GetComponent<Rigidbody>());
                Destroy(menu.GetComponent<BoxCollider>());
                Destroy(menu.GetComponent<Renderer>());
                menu.transform.localScale = new Vector3(0.1f, 0.3f, 0.3825f);

            // Menu Background
                menuBackground = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(menuBackground.GetComponent<Rigidbody>());
                Destroy(menuBackground.GetComponent<BoxCollider>());
                menuBackground.transform.parent = menu.transform;
                menuBackground.transform.rotation = Quaternion.identity;
                menuBackground.transform.localScale = menuSize;
                menuBackground.GetComponent<Renderer>().material.color = backgroundColor.colors[0].color;
                menuBackground.transform.position = new Vector3(0.05f, 0f, 0f);
                RoundObject(menuBackground);

                ColorChanger colorChanger = menuBackground.AddComponent<ColorChanger>();
                colorChanger.colors = backgroundColor;

            // Canvas
                canvasObject = new GameObject();
                canvasObject.transform.parent = menu.transform;
                Canvas canvas = canvasObject.AddComponent<Canvas>();
                CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
                canvasObject.AddComponent<GraphicRaycaster>();
                canvas.renderMode = RenderMode.WorldSpace;
                canvasScaler.dynamicPixelsPerUnit = 1000f;

            // Title and FPS
                Text text = new GameObject
                {
                    transform =
                    {
                        parent = canvasObject.transform
                    }
                }.AddComponent<Text>();
                text.font = currentFont;
                text.text = PluginInfo.Name + " <color=grey>[</color><color=white>" + (pageNumber + 1).ToString() + "</color><color=grey>]</color>";
                text.fontSize = 1;
                text.AddComponent<UIColorChanger>().colors = textColors[0];
            text.supportRichText = true;
                text.fontStyle = FontStyle.Italic;
                text.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 0;
                RectTransform component = text.GetComponent<RectTransform>();
                component.localPosition = Vector3.zero;
                component.sizeDelta = new Vector2(0.28f, 0.05f);
                component.position = new Vector3(0.06f, 0f, 0.165f);
                component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

                if (fpsCounter)
                {
                    fpsObject = new GameObject
                    {
                        transform =
                        {
                            parent = canvasObject.transform
                        }
                    }.AddComponent<Text>();
                    fpsObject.font = currentFont;
                    fpsObject.text = "FPS: " + Mathf.Ceil(1f / Time.unscaledDeltaTime).ToString();
                    fpsObject.AddComponent<UIColorChanger>().colors = textColors[0];
                fpsObject.fontSize = 1;
                    fpsObject.supportRichText = true;
                    fpsObject.fontStyle = FontStyle.Italic;
                    fpsObject.alignment = TextAnchor.MiddleCenter;
                    fpsObject.horizontalOverflow = HorizontalWrapMode.Overflow;
                    fpsObject.resizeTextForBestFit = true;
                    fpsObject.resizeTextMinSize = 0;
                    RectTransform component2 = fpsObject.GetComponent<RectTransform>();
                    component2.localPosition = Vector3.zero;
                    component2.sizeDelta = new Vector2(0.28f, 0.02f);
                    component2.position = new Vector3(0.06f, 0f, 0.135f);
                    component2.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
                }

            // Buttons
                // Disconnect
                    if (disconnectButton)
                    {
                        GameObject disconnectbutton = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        if (!UnityInput.Current.GetKey(keyboardButton))
                            disconnectbutton.layer = 2;
                        Destroy(disconnectbutton.GetComponent<Rigidbody>());
                        disconnectbutton.GetComponent<BoxCollider>().isTrigger = true;
                        disconnectbutton.transform.parent = menu.transform;
                        disconnectbutton.transform.rotation = Quaternion.identity;
                        disconnectbutton.transform.localScale = new Vector3(0.09f, 0.9f, 0.08f);
                        disconnectbutton.transform.localPosition = new Vector3(0.56f, 0f, 0.6f);
                        disconnectbutton.GetComponent<Renderer>().material.color = buttonColors[0].colors[0].color;
                        disconnectbutton.AddComponent<Classes.ButtonCollider>().relatedText = "Disconnect";

                        colorChanger = disconnectbutton.AddComponent<ColorChanger>();
                        colorChanger.colors = buttonColors[0];

                        Text discontext = new GameObject
                        {
                            transform =
                            {
                                parent = canvasObject.transform
                            }
                        }.AddComponent<Text>();
                        discontext.text = "Disconnect";
                        discontext.font = currentFont;
                        discontext.fontSize = 1;
                        discontext.AddComponent<UIColorChanger>().colors = textColors[0];
                discontext.alignment = TextAnchor.MiddleCenter;
                        discontext.resizeTextForBestFit = true;
                        discontext.resizeTextMinSize = 0;

                        RectTransform rectt = discontext.GetComponent<RectTransform>();
                        rectt.localPosition = Vector3.zero;
                        rectt.sizeDelta = new Vector2(0.2f, 0.03f);
                        rectt.localPosition = new Vector3(0.064f, 0f, 0.23f);
                        rectt.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
                    }

                // Page Buttons
                    GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (!UnityInput.Current.GetKey(keyboardButton))
                        gameObject.layer = 2;
                    Destroy(gameObject.GetComponent<Rigidbody>());
                    gameObject.GetComponent<BoxCollider>().isTrigger = true;
                    gameObject.transform.parent = menu.transform;
                    gameObject.transform.rotation = Quaternion.identity;
                    gameObject.transform.localScale = new Vector3(0.09f, menuSize.y * 0.5f, 0.1f);
                    gameObject.transform.localPosition = new Vector3(0.56f, 0.25f, -0.6f);
                    gameObject.GetComponent<Renderer>().material.color = buttonColors[0].colors[0].color;
                    gameObject.AddComponent<Classes.ButtonCollider>().relatedText = "PreviousPage";

                    colorChanger = gameObject.AddComponent<ColorChanger>();
                    colorChanger.colors = buttonColors[0];

                    text = new GameObject
                    {
                        transform =
                        {
                            parent = canvasObject.transform
                        }
                    }.AddComponent<Text>();
                    text.font = currentFont;
                    text.text = "<";
                    text.fontSize = 1;
                    text.AddComponent<UIColorChanger>().colors = textColors[0];
                    text.alignment = TextAnchor.MiddleCenter;
                    text.resizeTextForBestFit = true;
                    text.resizeTextMinSize = 0;
                    component = text.GetComponent<RectTransform>();
                    component.localPosition = Vector3.zero;
                    component.sizeDelta = new Vector2(0.2f, 0.03f);
                    component.localPosition = canvasObject.transform.InverseTransformPoint(gameObject.transform.position) + Vector3.right * 0.01f;
                    component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

                    gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (!UnityInput.Current.GetKey(keyboardButton))
                    {
                        gameObject.layer = 2;
                    }
                    Destroy(gameObject.GetComponent<Rigidbody>());
                    gameObject.GetComponent<BoxCollider>().isTrigger = true;
                    gameObject.transform.parent = menu.transform;
                    gameObject.transform.rotation = Quaternion.identity;
                    gameObject.transform.localScale = new Vector3(0.09f, menuSize.y * 0.5f, 0.1f);
                    gameObject.transform.localPosition = new Vector3(0.56f, -0.25f, -0.6f);
                    gameObject.GetComponent<Renderer>().material.color = buttonColors[0].colors[0].color;
                    gameObject.AddComponent<Classes.ButtonCollider>().relatedText = "NextPage";

                    colorChanger = gameObject.AddComponent<ColorChanger>();
                    colorChanger.colors = buttonColors[0];

                    text = new GameObject
                    {
                        transform =
                        {
                            parent = canvasObject.transform
                        }
                    }.AddComponent<Text>();
                    text.font = currentFont;
                    text.text = ">";
                    text.fontSize = 1;
                    text.AddComponent<UIColorChanger>().colors = textColors[0];
                    text.alignment = TextAnchor.MiddleCenter;
                    text.resizeTextForBestFit = true;
                    text.resizeTextMinSize = 0;
                    component = text.GetComponent<RectTransform>();
                    component.localPosition = Vector3.zero;
                    component.sizeDelta = new Vector2(0.2f, 0.03f);
                    component.localPosition = canvasObject.transform.InverseTransformPoint(gameObject.transform.position) + Vector3.right * 0.01f;
                    component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

                    // Category Sidebar
                    int categoryCount = categories.Length;

                    sidebar = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Destroy(sidebar.GetComponent<Rigidbody>());
                    Destroy(sidebar.GetComponent<BoxCollider>());
                    sidebar.transform.parent = menu.transform;
                    sidebar.transform.rotation = Quaternion.identity;
                    sidebar.transform.localScale = menuSize - Vector3.up * 0.35f;
                    sidebar.GetComponent<Renderer>().material.color = backgroundColor.colors[0].color;
                    sidebar.transform.position = new Vector3(0.05f, -0.285f, 0f);
                    RoundObject(sidebar);

                    ColorChanger sidebarColor = sidebar.AddComponent<ColorChanger>();
                    sidebarColor.colors = backgroundColor;

            for (int i = 0; i < categoryCount; i++)
                    {
                        Category category = categories[i];

                        CreateCategoryButton(
                            0.41f - (i * 0.1f),
                            category
                        );
                    }

            // Mod Buttons
            List<object> menuItems = new List<object>();

            // Subcategories get priority
            foreach (Category subcategory in currentCategory.Subcategories)
                menuItems.Add(subcategory);

            // Normal buttons come after subcategories
            foreach (ButtonInfo button in currentCategory.Buttons)
                menuItems.Add(button);

            object[] activeItems = menuItems
                .Skip(pageNumber * buttonsPerPage)
                .Take(buttonsPerPage)
                .ToArray();

            for (int i = 0; i < activeItems.Length; i++)
            {
                if (activeItems[i] is Category subcategory)
                    CreateSubcategoryButton(i * 0.1f, subcategory);
                else if (activeItems[i] is ButtonInfo button)
                    CreateButton(i * 0.1f, button, i);
            }
        }

        public static void CreateSubcategoryButton(float offset, Category category)
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);

            if (!UnityInput.Current.GetKey(keyboardButton))
                gameObject.layer = 2;

            Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(0.09f, 0.9f, 0.08f);
            gameObject.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - offset);

            gameObject.GetComponent<Renderer>().material.color =
                buttonColors[0].colors[0].color;

            gameObject.AddComponent<Classes.ButtonCollider>().relatedText =
                "Subcategory:" + category.Name;

            ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
            colorChanger.colors = buttonColors[0];

            Text text = new GameObject
            {
                transform =
        {
            parent = canvasObject.transform
        }
            }.AddComponent<Text>();

            text.font = currentFont;
            text.text = category.Name;
            text.supportRichText = true;
            text.fontSize = 1;
            text.AddComponent<UIColorChanger>().colors = textColors[0];
            text.alignment = TextAnchor.MiddleCenter;
            text.fontStyle = FontStyle.Italic;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;

            RectTransform component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(.2f, .03f);
            component.localPosition = new Vector3(.064f, 0, .111f - offset / 2.6f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
        }

        public static void CreateButton(float offset, ButtonInfo method, int buttonIndex)
        {
            if (method != null && !method.label)
            {
                GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (!UnityInput.Current.GetKey(keyboardButton))
                    gameObject.layer = 2;

                Destroy(gameObject.GetComponent<Rigidbody>());
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.transform.parent = menu.transform;
                gameObject.transform.rotation = Quaternion.identity;
                gameObject.transform.localScale = new Vector3(0.09f, 0.9f, 0.08f);
                gameObject.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - offset);
                ButtonCollider Button = gameObject.AddComponent<Classes.ButtonCollider>();
                Button.relatedText = method.buttonText;

                if (method.incremental)
                {
                    gameObject.transform.localScale -= new Vector3(0f, 0.254f, 0f);
                    Destroy(Button);

                    RenderIncrementalButton(false, offset, buttonIndex, method);
                    RenderIncrementalButton(true, offset, buttonIndex, method);
                }

                ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
                colorChanger.colors = method.enabled ? buttonColors[1] : buttonColors[0];
            }
            Text text = new GameObject
            {
                transform =
                {
                    parent = canvasObject.transform
                }
            }.AddComponent<Text>();
            text.font = currentFont;
            text.text = method.buttonText;

            if (method.overlapText != null)
                text.text = method.overlapText;

            text.supportRichText = true;
            text.fontSize = 1;
            text.AddComponent<UIColorChanger>().colors = textColors[0];
            text.alignment = TextAnchor.MiddleCenter;
            text.fontStyle = FontStyle.Italic;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            RectTransform component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(.2f, .03f);
            component.localPosition = new Vector3(.064f, 0, .111f - offset / 2.6f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            
        }

        private static void RenderIncrementalButton(bool increment, float offset, int buttonIndex, ButtonInfo method)
        {
            if (!method.label)
            {
                GameObject buttonObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (UnityInput.Current.GetKey(keyboardButton))
                buttonObject.layer = 2;

                buttonObject.GetComponent<BoxCollider>().isTrigger = true;
                buttonObject.transform.parent = menu.transform;
                buttonObject.transform.rotation = Quaternion.identity;

                buttonObject.transform.localScale = new Vector3(0.09f, 0.102f, offset * 0.8f);
                buttonObject.transform.localPosition = new Vector3(0.56f, 0.599f, 0.28f - offset);

                ButtonCollider button = buttonObject.AddComponent<ButtonCollider>();
                button.relatedText = method.buttonText;
                button.incremental = true;
                button.positive = increment;

                if (increment)
                    buttonObject.transform.localPosition = new Vector3(buttonObject.transform.localPosition.x, -buttonObject.transform.localPosition.y, buttonObject.transform.localPosition.z);

                if (lastClickedName != method.buttonText + (increment ? "+" : "-"))
                {
                    ColorChanger colorChanger = buttonObject.AddComponent<ColorChanger>();
                    colorChanger.colors = buttonColors[0];
                }
                else
                    CoroutineManager.instance.StartCoroutine(ButtonClick(buttonIndex, buttonObject.GetComponent<Renderer>()));

                //FollowMenuSettings(buttonObject);
            }

            RenderIncrementalText(increment, offset);
        }

        public static void RenderIncrementalText(bool increment, float offset)
        {
            TextMeshPro buttonText = new GameObject
            {
                transform =
                {
                    parent = canvasObject.transform
                }
            }.AddComponent<TextMeshPro>();

            buttonText.font = activeFont;
            buttonText.text = increment ? "+" : "-";
            buttonText.richText = true;
            buttonText.fontSize = 1;
            buttonText.AddComponent<UIColorChanger>().colors = textColors[1];

            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.fontStyle = activeFontStyle;
            buttonText.enableAutoSizing = true;
            buttonText.fontSizeMin = 0;

            RectTransform textTransform = buttonText.GetComponent<RectTransform>();
            textTransform.localPosition = Vector3.zero;
            textTransform.sizeDelta = new Vector2(.2f, .03f * (offset / 0.1f));

            textTransform.localPosition = new Vector3(.064f, increment ? -0.18f : 0.18f, .111f - offset / 2.6f);
            textTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            //FollowMenuSettings(buttonText);
        }

        public static IEnumerator ButtonClick(int buttonIndex, Renderer render)
        {
            lastClickedName = "";
            float elapsedTime = 0f;
            while (elapsedTime < 0.1f)
            {
                int from = 1;
                int to = 1 - from;

                render.material.color = Color.Lerp(
                    buttonColors[from].GetCurrentColor(),
                    buttonColors[to].GetCurrentColor(),
                    elapsedTime / 0.1f
                );

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            ColorChanger colorChanger = render.gameObject.AddComponent<ColorChanger>();
            colorChanger.colors = buttonColors[0];
            render.enabled = false;

            ExtGradient gradient = colorChanger.colors.Clone();
            gradient.SetColor(0, Color.red);

            colorChanger.colors = gradient;
        }

        public static void RoundMenuObject(GameObject toRound, float Bevel = 0.02f)
        {
            if (toRound.transform.parent != menu?.transform)
            {
                RoundObject(toRound, Bevel);
                return;
            }

            Renderer ToRoundRenderer = toRound.GetComponent<Renderer>();
            GameObject BaseA = GameObject.CreatePrimitive(PrimitiveType.Cube);
            BaseA.GetComponent<Renderer>().enabled = ToRoundRenderer.enabled;
            Destroy(BaseA.GetComponent<Collider>());

            BaseA.transform.parent = menu.transform;
            BaseA.transform.rotation = Quaternion.identity;
            BaseA.transform.localPosition = toRound.transform.localPosition;
            BaseA.transform.localScale = toRound.transform.localScale + new Vector3(0f, Bevel * -2.55f, 0f);

            GameObject BaseB = GameObject.CreatePrimitive(PrimitiveType.Cube);
            BaseB.GetComponent<Renderer>().enabled = ToRoundRenderer.enabled;
            Destroy(BaseB.GetComponent<Collider>());

            BaseB.transform.parent = menu.transform;
            BaseB.transform.rotation = Quaternion.identity;
            BaseB.transform.localPosition = toRound.transform.localPosition;
            BaseB.transform.localScale = toRound.transform.localScale + new Vector3(0f, 0f, -Bevel * 2f);

            GameObject RoundCornerA = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RoundCornerA.GetComponent<Renderer>().enabled = ToRoundRenderer.enabled;
            Destroy(RoundCornerA.GetComponent<Collider>());

            RoundCornerA.transform.parent = menu.transform;
            RoundCornerA.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 0f, 90f);

            RoundCornerA.transform.localPosition = toRound.transform.localPosition + new Vector3(0f, toRound.transform.localScale.y / 2f - Bevel * 1.275f, toRound.transform.localScale.z / 2f - Bevel);
            RoundCornerA.transform.localScale = new Vector3(Bevel * 2.55f, toRound.transform.localScale.x / 2f, Bevel * 2f);

            GameObject RoundCornerB = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RoundCornerB.GetComponent<Renderer>().enabled = ToRoundRenderer.enabled;
            Destroy(RoundCornerB.GetComponent<Collider>());

            RoundCornerB.transform.parent = menu.transform;
            RoundCornerB.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 0f, 90f);

            RoundCornerB.transform.localPosition = toRound.transform.localPosition + new Vector3(0f, -(toRound.transform.localScale.y / 2f) + Bevel * 1.275f, toRound.transform.localScale.z / 2f - Bevel);
            RoundCornerB.transform.localScale = new Vector3(Bevel * 2.55f, toRound.transform.localScale.x / 2f, Bevel * 2f);

            GameObject RoundCornerC = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RoundCornerC.GetComponent<Renderer>().enabled = ToRoundRenderer.enabled;
            Destroy(RoundCornerC.GetComponent<Collider>());

            RoundCornerC.transform.parent = menu.transform;
            RoundCornerC.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 0f, 90f);

            RoundCornerC.transform.localPosition = toRound.transform.localPosition + new Vector3(0f, toRound.transform.localScale.y / 2f - Bevel * 1.275f, -(toRound.transform.localScale.z / 2f) + Bevel);
            RoundCornerC.transform.localScale = new Vector3(Bevel * 2.55f, toRound.transform.localScale.x / 2f, Bevel * 2f);

            GameObject RoundCornerD = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RoundCornerD.GetComponent<Renderer>().enabled = ToRoundRenderer.enabled;
            Destroy(RoundCornerD.GetComponent<Collider>());

            RoundCornerD.transform.parent = menu.transform;
            RoundCornerD.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 0f, 90f);

            RoundCornerD.transform.localPosition = toRound.transform.localPosition + new Vector3(0f, -(toRound.transform.localScale.y / 2f) + Bevel * 1.275f, -(toRound.transform.localScale.z / 2f) + Bevel);
            RoundCornerD.transform.localScale = new Vector3(Bevel * 2.55f, toRound.transform.localScale.x / 2f, Bevel * 2f);

            GameObject[] ToChange = {
                BaseA,
                BaseB,
                RoundCornerA,
                RoundCornerB,
                RoundCornerC,
                RoundCornerD
            };

            foreach (GameObject Changed in ToChange)
            {
                ClampColor TargetChanger = Changed.AddComponent<ClampColor>();
                TargetChanger.targetRenderer = ToRoundRenderer;
            }

            ToRoundRenderer.enabled = false;

            //ColorChanger colorChanger = ToRoundRenderer.GetComponent<ColorChanger>();
            //if (colorChanger)
            //    colorChanger.overrideTransparency = false;
        }

        /// <summary>
        /// Replaces the specified GameObject with a visually rounded version by constructing a composite of primitive
        /// shapes with beveled edges.
        /// </summary>
        /// <remarks>This method disables the original object's Renderer and creates new child primitives
        /// to approximate a rounded appearance. The original object's ColorChanger component, if present, will have its
        /// overrideTransparency property set to false. The method does not modify the original object's collider or
        /// mesh, and is intended for visual effects only.</remarks>
        /// <param name="toRound">The GameObject to be visually rounded. Must have a Renderer component attached.</param>
        /// <param name="bevel">The width, in world units, of the bevel applied to the object's edges. Must be non-negative. The default
        /// value is 0.02.</param>
        public static void RoundObject(GameObject toRound, float bevel = 0.02f)
        {
            static GameObject CreatePrimitive(PrimitiveType type, Transform parent, bool rendererEnabled)
            {
                GameObject obj = GameObject.CreatePrimitive(type);
                obj.GetComponent<Renderer>().enabled = rendererEnabled;

                Collider collider = obj.GetComponent<Collider>();
                if (collider != null)
                    Destroy(collider);

                obj.transform.SetParent(parent, false);
                return obj;
            }

            Renderer renderer = toRound.GetComponent<Renderer>();
            if (renderer == null) return;

            Transform parent = toRound.transform;
            Vector3 scale = parent.localScale;
            bool rendererEnabled = renderer.enabled;

            GameObject baseA = CreatePrimitive(PrimitiveType.Cube, parent, rendererEnabled);
            baseA.transform.localPosition = Vector3.zero;
            baseA.transform.localRotation = Quaternion.identity;
            baseA.transform.localScale = new Vector3(scale.x, scale.y - bevel * 2f, scale.z);

            GameObject baseB = CreatePrimitive(PrimitiveType.Cube, parent, rendererEnabled);
            baseB.transform.localPosition = Vector3.zero;
            baseB.transform.localRotation = Quaternion.identity;
            baseB.transform.localScale = new Vector3(scale.x, scale.y, scale.z - bevel * 2f);

            GameObject[] corners = new GameObject[4];
            Vector3[] cornerOffsets = {
                new Vector3(0f, scale.y / 2f - bevel, scale.z / 2f - bevel),
                new Vector3(0f, -scale.y / 2f + bevel, scale.z / 2f - bevel),
                new Vector3(0f, scale.y / 2f - bevel, -scale.z / 2f + bevel),
                new Vector3(0f, -scale.y / 2f + bevel, -scale.z / 2f + bevel)
            };

            for (int i = 0; i < 4; i++)
            {
                corners[i] = CreatePrimitive(PrimitiveType.Cylinder, parent, rendererEnabled);
                corners[i].transform.localPosition = cornerOffsets[i];
                corners[i].transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
                corners[i].transform.localScale = new Vector3(bevel * 2f, scale.x / 2f, bevel * 2f);
            }

            GameObject[] allObjects = { baseA, baseB, corners[0], corners[1], corners[2], corners[3] };
            foreach (GameObject obj in allObjects)
            {
                ClampColor clampColor = obj.AddComponent<ClampColor>();
                clampColor.targetRenderer = renderer;
            }

            renderer.enabled = false;

            //ColorChanger colorChanger = renderer.GetComponent<ColorChanger>();
            //if (colorChanger != null)
            //    colorChanger.overrideTransparency = false;
        }

        public static void ToggleIncremental(string buttonText, bool increment, bool reload = true)
        {
            ButtonInfo target = Buttons.GetIndex(buttonText);
            if (target != null)
            {
                string newIndicator = " <color=grey>[</color><color=green>New</color><color=grey>]</color>";
                if (target.overlapText != null && target.overlapText.Contains(newIndicator))
                {
                    target.overlapText = target.overlapText.Replace(newIndicator, "");
                    if (target.overlapText == target.buttonText)
                        target.overlapText = target.buttonText;
                }

                if (target.label)
                    return;

                //bool boost = incrementalBoost && rightGrab;
                if (increment)
                {
                    NotifiLib.SendNotification($"<color=grey>[</color><color=green>INCREMENT</color><color=grey>]</color> {target.toolTip}");

                    //if (boost)
                    //    for (int i = 0; i < 5; i++)
                    //    {
                    //        if (target.enableMethod == null) continue;
                    //        try { target.enableMethod.Invoke(); }
                    //        catch (Exception exc)
                    //        {
                    //            Debug.LogError(
                    //                $"Error with mod enableMethod {target.buttonText} at {exc.StackTrace}: {exc.Message}");
                    //        }
                    //    }
                    //else
                        if (target.enableMethod != null)
                        try { target.enableMethod.Invoke(); }
                        catch (Exception exc)
                        {
                            Debug.LogError(
                            $"Error with mod enableMethod {target.buttonText} at {exc.StackTrace}: {exc.Message}");
                        }
                }
                else
                {
                    NotifiLib.SendNotification($"<color=grey>[</color><color=red>DECREMENT</color><color=grey>]</color> {target.toolTip}");

                    //if (boost)
                    //    for (int i = 0; i < 5; i++)
                    //    {
                    //        if (target.enableMethod == null) continue;
                    //        if (target.disableMethod == null) continue;
                    //        try { target.disableMethod.Invoke(); }
                    //        catch (Exception exc)
                    //        {
                    //            Debug.LogError(
                    //                $"Error with mod disableMethod {target.buttonText} at {exc.StackTrace}: {exc.Message}");
                    //        }
                    //    }
                    //else
                        if (target.disableMethod != null)
                        try { target.disableMethod.Invoke(); }
                        catch (Exception exc)
                        {
                            Debug.LogError(
                            $"Error with mod disableMethod {target.buttonText} at {exc.StackTrace}: {exc.Message}");
                        }
                }
            }
            else
                Debug.LogError($"{buttonText} does not exist");
        }

        public static void RecreateMenu()
        {
            if (menu != null)
            {
                Destroy(menu);
                menu = null;

                CreateMenu();
                RecenterMenu(rightHanded, UnityInput.Current.GetKey(keyboardButton));
            }
        }

        public static void RecenterMenu(bool isRightHanded, bool isKeyboardCondition)
        {
            if (!isKeyboardCondition)
            {
                if (!isRightHanded)
                {
                    menu.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                    menu.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
                }
                else
                {
                    menu.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                    Vector3 rotation = GorillaTagger.Instance.rightHandTransform.rotation.eulerAngles;
                    rotation += new Vector3(0f, 0f, 180f);
                    menu.transform.rotation = Quaternion.Euler(rotation);
                }
            }
            else
            {
                try
                {
                    TPC = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera").GetComponent<Camera>();
                }
                catch { }

                GameObject.Find("Shoulder Camera").transform.Find("CM vcam1").gameObject.SetActive(false);

                if (TPC != null)
                {
                    TPC.transform.position = new Vector3(-999f, -999f, -999f);
                    TPC.transform.rotation = Quaternion.identity;
                    GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bg.transform.localScale = new Vector3(10f, 10f, 0.01f);
                    bg.transform.transform.position = TPC.transform.position + TPC.transform.forward;
                    Color realcolor = backgroundColor.GetCurrentColor();
                    bg.GetComponent<Renderer>().material.color = new Color32((byte)(realcolor.r * 50), (byte)(realcolor.g * 50), (byte)(realcolor.b * 50), 255);
                    Destroy(bg, 0.05f);
                    menu.transform.parent = TPC.transform;
                    menu.transform.position = TPC.transform.position + (TPC.transform.forward * 0.5f) + (TPC.transform.up * -0.02f);
                    menu.transform.rotation = TPC.transform.rotation * Quaternion.Euler(-90f, 90f, 0f);

                    if (reference != null)
                    {
                        if (Mouse.current.leftButton.isPressed)
                        {
                            Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                            bool hitButton = Physics.Raycast(ray, out RaycastHit hit, 100);
                            if (hitButton)
                            {
                                Classes.ButtonCollider collide = hit.transform.gameObject.GetComponent<Classes.ButtonCollider>();
                                collide?.OnTriggerEnter(buttonCollider);
                            }
                        } 
                        else
                            reference.transform.position = new Vector3(999f, -999f, -999f);
                    }
                }
            }
        }

        public static void CreateReference(bool isRightHanded)
        {
            reference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            reference.transform.parent = isRightHanded ? GorillaTagger.Instance.leftHandTransform : GorillaTagger.Instance.rightHandTransform;
            reference.GetComponent<Renderer>().material.color = backgroundColor.colors[0].color;
            reference.transform.localPosition = new Vector3(0f, -0.1f, 0f);
            reference.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            buttonCollider = reference.GetComponent<SphereCollider>();

            ColorChanger colorChanger = reference.AddComponent<ColorChanger>();
            colorChanger.colors = backgroundColor;
        }

        public static void Toggle(string buttonText)
        {
            if (buttonText.StartsWith("Subcategory:"))
            {
                string categoryName =
                    buttonText.Substring("Subcategory:".Length);

                Category subcategory = currentCategory.GetSubcategory(categoryName);

                if (subcategory != null)
                {
                    currentCategory = subcategory;
                    pageNumber = 0;
                }

                RecreateMenu();
                return;
            }

            if (buttonText.StartsWith("Category:"))
            {
                string categoryName =
                    buttonText.Substring("Category:".Length);

                Category category = FindCategory(categoryName);

                if (category != null)
                {
                    currentCategory = category;
                    pageNumber = 0;
                }

                RecreateMenu();
                return;
            }

            int itemCount =
            currentCategory.Subcategories.Count +
            currentCategory.Buttons.Count;

            int lastPage =
                Mathf.Max(0, (itemCount + buttonsPerPage - 1) / buttonsPerPage - 1);

            if (buttonText == "PreviousPage")
            {
                pageNumber--;

                if (pageNumber < 0)
                    pageNumber = lastPage;
            }
            else if (buttonText == "NextPage")
            {
                pageNumber++;

                if (pageNumber > lastPage)
                    pageNumber = 0;
            }
            else
            {
                ButtonInfo target = GetIndex(buttonText);

                if (target != null)
                {
                    if (target.isTogglable)
                    {
                        target.enabled = !target.enabled;

                        if (target.enabled)
                        {
                            NotifiLib.SendNotification(
                                "<color=grey>[</color><color=green>ENABLE</color><color=grey>]</color> " +
                                target.toolTip
                            );

                            if (target.enableMethod != null)
                            {
                                try
                                {
                                    target.enableMethod.Invoke();
                                }
                                catch (Exception exc)
                                {
                                    Debug.LogError(
                                        $"{PluginInfo.Name} // Error enabling {target.buttonText}: {exc}"
                                    );
                                }
                            }
                        }
                        else
                        {
                            NotifiLib.SendNotification(
                                "<color=grey>[</color><color=red>DISABLE</color><color=grey>]</color> " +
                                target.toolTip
                            );

                            if (target.disableMethod != null)
                            {
                                try
                                {
                                    target.disableMethod.Invoke();
                                }
                                catch (Exception exc)
                                {
                                    Debug.LogError(
                                        $"{PluginInfo.Name} // Error disabling {target.buttonText}: {exc}"
                                    );
                                }
                            }
                        }
                    }
                    else
                    {
                        NotifiLib.SendNotification(
                            "<color=grey>[</color><color=green>ENABLE</color><color=grey>]</color> " +
                            target.toolTip
                        );

                        if (target.method != null)
                        {
                            try
                            {
                                target.method.Invoke();
                            }
                            catch (Exception exc)
                            {
                                Debug.LogError(
                                    $"{PluginInfo.Name} // Error executing {target.buttonText}: {exc}"
                                );
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError(buttonText + " does not exist");
                }
            }

            RecreateMenu();
        }

        public static Category FindCategory(string name)
        {
            foreach (Category category in categories)
            {
                Category result = FindCategoryRecursive(category, name);

                if (result != null)
                    return result;
            }

            return null;
        }

        private static Category FindCategoryRecursive(
            Category category,
            string name)
        {
            if (category.Name == name)
                return category;

            foreach (Category subcategory in category.Subcategories)
            {
                Category result =
                    FindCategoryRecursive(subcategory, name);

                if (result != null)
                    return result;
            }

            return null;
        }

        public static Vector3 RandomVector3(float range = 1f) =>
            new Vector3(UnityEngine.Random.Range(-range, range),
                        UnityEngine.Random.Range(-range, range),
                        UnityEngine.Random.Range(-range, range));

        public static Quaternion RandomQuaternion(float range = 360f) =>
            Quaternion.Euler(UnityEngine.Random.Range(0f, range),
                        UnityEngine.Random.Range(0f, range),
                        UnityEngine.Random.Range(0f, range));

        public static Color RandomColor(byte range = 255, byte alpha = 255) =>
            new Color32((byte)UnityEngine.Random.Range(0, range),
                        (byte)UnityEngine.Random.Range(0, range),
                        (byte)UnityEngine.Random.Range(0, range),
                        alpha);

        public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) TrueLeftHand()
        {
            Quaternion rot = GorillaTagger.Instance.leftHandTransform.rotation * GTPlayer.Instance.LeftHand.handRotOffset;
            return (GorillaTagger.Instance.leftHandTransform.position + GorillaTagger.Instance.leftHandTransform.rotation * GTPlayer.Instance.LeftHand.handOffset, rot, rot * Vector3.up, rot * Vector3.forward, rot * Vector3.right);
        }

        public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) TrueRightHand()
        {
            Quaternion rot = GorillaTagger.Instance.rightHandTransform.rotation * GTPlayer.Instance.RightHand.handRotOffset;
            return (GorillaTagger.Instance.rightHandTransform.position + GorillaTagger.Instance.rightHandTransform.rotation * GTPlayer.Instance.RightHand.handOffset, rot, rot * Vector3.up, rot * Vector3.forward, rot * Vector3.right);
        }

        public static void WorldScale(GameObject obj, Vector3 targetWorldScale)
        {
            Vector3 parentScale = obj.transform.parent.lossyScale;
            obj.transform.localScale = new Vector3(
                targetWorldScale.x / parentScale.x,
                targetWorldScale.y / parentScale.y,
                targetWorldScale.z / parentScale.z
            );
        }

        public static void FixStickyColliders(GameObject platform)
        {
            Vector3[] localPositions = new Vector3[]
            {
                new Vector3(0, 1f, 0),
                new Vector3(0, -1f, 0),
                new Vector3(1f, 0, 0),
                new Vector3(-1f, 0, 0),
                new Vector3(0, 0, 1f),
                new Vector3(0, 0, -1f)
            };
            Quaternion[] localRotations = new Quaternion[]
            {
                Quaternion.Euler(90, 0, 0),
                Quaternion.Euler(-90, 0, 0),
                Quaternion.Euler(0, -90, 0),
                Quaternion.Euler(0, 90, 0),
                Quaternion.identity,
                Quaternion.Euler(0, 180, 0)
            };
            for (int i = 0; i < localPositions.Length; i++)
            {
                GameObject side = GameObject.CreatePrimitive(PrimitiveType.Cube);
                try
                {
                    if (platform.GetComponent<GorillaSurfaceOverride>() != null)
                    {
                        side.AddComponent<GorillaSurfaceOverride>().overrideIndex = platform.GetComponent<GorillaSurfaceOverride>().overrideIndex;
                    }
                }
                catch { }
                float size = 0.025f;
                side.transform.SetParent(platform.transform);
                side.transform.position = localPositions[i] * (size / 2);
                side.transform.rotation = localRotations[i];
                WorldScale(side, new Vector3(size, size, 0.01f));
                side.GetComponent<Renderer>().enabled = false;
            }
        }

        public static IEnumerable<ButtonInfo> GetAllButtons(Category category)
        {
            foreach (ButtonInfo button in category.Buttons)
                yield return button;

            foreach (Category subcategory in category.Subcategories)
            {
                foreach (ButtonInfo button in GetAllButtons(subcategory))
                    yield return button;
            }
        }

        public static void CreateCategoryButton(float offset, Category category, bool hotbar = false)
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);

            if (!UnityInput.Current.GetKey(keyboardButton))
                gameObject.layer = 2;

            Destroy(gameObject.GetComponent<Rigidbody>());

            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;

            gameObject.transform.localScale =
                new Vector3(0.09f, 0.6f, 0.08f);

            gameObject.transform.localPosition =
                new Vector3(0.65f, -0.95f, offset);

            gameObject.GetComponent<Renderer>().material.color =
                category == currentCategory
                    ? buttonColors[1].colors[0].color
                    : buttonColors[0].colors[0].color;

            gameObject.AddComponent<Classes.ButtonCollider>().relatedText =
                "Category:" + category.Name;

            ColorChanger colorChanger =
                gameObject.AddComponent<ColorChanger>();

            colorChanger.colors =
                category == currentCategory
                    ? buttonColors[1]
                    : buttonColors[0];

            Text text = new GameObject
            {
                transform =
        {
            parent = canvasObject.transform
        }
            }.AddComponent<Text>();

            text.font = currentFont;
            text.text = category.Name;
            text.fontSize = 1;
            text.AddComponent<UIColorChanger>().colors = category == currentCategory ? textColors[1] : textColors[0];

            text.alignment = TextAnchor.MiddleCenter;
            text.fontStyle = FontStyle.Italic;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;

            RectTransform rect =
                text.GetComponent<RectTransform>();

            rect.localPosition = Vector3.zero;
            rect.sizeDelta = new Vector2(0.2f, 0.03f);

            rect.localPosition = canvasObject.transform.InverseTransformPoint(gameObject.transform.position) + Vector3.right * 0.01f;

            rect.rotation =
                Quaternion.Euler(180f, 90f, 90f);
        }

        private static int? noInvisLayerMask;
        public static int NoInvisLayerMask()
        {
            noInvisLayerMask ??= ~(
                1 << LayerMask.NameToLayer("TransparentFX") |
                1 << LayerMask.NameToLayer("Ignore Raycast") |
                1 << LayerMask.NameToLayer("Zone") |
                1 << LayerMask.NameToLayer("Gorilla Trigger") |
                1 << LayerMask.NameToLayer("Gorilla Boundary") |
                1 << LayerMask.NameToLayer("GorillaCosmetics") |
                1 << LayerMask.NameToLayer("GorillaParticle"));

            return noInvisLayerMask ?? GTPlayer.Instance.locomotionEnabledLayers;
        }

        public static bool gunLocked;
        public static VRRig lockTarget;

        public static (RaycastHit Ray, GameObject NewPointer) RenderGun(int? overrideLayerMask = null)
        {
            Transform GunTransform = GorillaTagger.Instance.rightHandTransform;

            Vector3 StartPosition = GunTransform.position;
            Vector3 Direction = GunTransform.forward;

            Physics.Raycast(StartPosition + Direction / 4f, Direction, out var Ray, 512f, overrideLayerMask ?? NoInvisLayerMask());
            Vector3 EndPosition = gunLocked ? lockTarget.transform.position : Ray.point;

            if (EndPosition == Vector3.zero)
                EndPosition = StartPosition + Direction * 512f;

            if (GunPointer == null)
                GunPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            GunPointer.SetActive(true);
            GunPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            GunPointer.transform.position = EndPosition;

            Renderer PointerRenderer = GunPointer.GetComponent<Renderer>();
            PointerRenderer.material.shader = Shader.Find("GUI/Text Shader");
            PointerRenderer.material.color = gunLocked || ControllerInputPoller.TriggerFloat(XRNode.RightHand) > 0.5f ? buttonColors[1].GetCurrentColor() : buttonColors[0].GetCurrentColor();

            Destroy(GunPointer.GetComponent<Collider>());

            if (GunLine == null)
            {
                GameObject line = new GameObject("iiMenu_GunLine");
                GunLine = line.AddComponent<LineRenderer>();
            }

            GunLine.gameObject.SetActive(true);
            GunLine.material.shader = Shader.Find("GUI/Text Shader");
            GunLine.startColor = backgroundColor.GetCurrentColor();
            GunLine.endColor = backgroundColor.GetCurrentColor(0.5f);
            GunLine.startWidth = 0.025f;
            GunLine.endWidth = 0.025f;
            GunLine.positionCount = 2;
            GunLine.useWorldSpace = true;

            GunLine.SetPosition(0, StartPosition);
            GunLine.SetPosition(1, EndPosition);

            return (Ray, GunPointer);
        }

        // Variables
        // Important
        // Objects
        public static GameObject menu;
        public static GameObject menuBackground;
        public static GameObject sidebar;
        public static GameObject hotbar;
        public static GameObject reference;
        public static GameObject canvasObject;
        public static TMP_FontAsset activeFont;
        public static FontStyles activeFontStyle = FontStyles.Normal;

        public static SphereCollider buttonCollider;
        public static Camera TPC;
        public static Text fpsObject;
        public static string lastClickedName = "";

        private static GameObject GunPointer;
        private static LineRenderer GunLine;

        // Data
        public static int pageNumber = 0;
        public static int pageOffset;
        public static Category currentCategory;
    }
}

using UnityEditor;
using UnityEngine;

namespace SharedAssets.GuiMain
{
    public class GuiMainEditor
    {
        private static string mRootGoName = "GuiInteraction";
        private static string mTopBarControllerName = "TopBarController";
        private static string mGuiObjectManagerName = "GuiObjectManager";
        private static string mDialogueName = "Dialogue";
        private static string mMenuObjectName = "MenuManager";

        private static GameObject mRoot = null;
        private static GameObject mGuiObjectManager = null;
        private static GameObject mMenuObject = null;

        [MenuItem("EcogotchiGUI/Add Dialogue")]
        private static void AddDialogue()
        {
            createRootIfNotExist();
            createGuiObjectManagerIfNotExist();

            GameObject newDialogue = new GameObject(mDialogueName);
            newDialogue.transform.parent = mRoot.transform;
            newDialogue.AddComponent<DialogueBehaviour>();
        }

        [MenuItem("EcogotchiGUI/Add TopBarController")]
        private static void AddTopBarController()
        {
            createRootIfNotExist();
            createGuiObjectManagerIfNotExist();

            //mTopBarController = GameObject.Find(mTopBarControllerName);
            //if (mTopBarController == null)
            //{
            //    mTopBarController = new GameObject(mTopBarControllerName);
            //    mTopBarController.transform.parent = mRoot.transform;
            //    mTopBarController.AddComponent<TopBarBehaviour>();
            //}

            GameObject newTopBarController = new GameObject(mTopBarControllerName);
            newTopBarController.transform.parent = mRoot.transform;
            newTopBarController.AddComponent<TopBarBehaviour>();
        }

        [MenuItem("EcogotchiGUI/Update")]
        private static void Update()
        {
            createRootIfNotExist();
            createGuiObjectManagerIfNotExist();
            createMenuObjectIfNotExist();
        }

        private static void createRootIfNotExist()
        {
            mRoot = GameObject.Find(mRootGoName);

            if (mRoot == null)
            {
                mRoot = new GameObject(mRootGoName);
            }
        }

        private static void createGuiObjectManagerIfNotExist()
        {
            mGuiObjectManager = GameObject.Find(mGuiObjectManagerName);
            
            if (mGuiObjectManager == null)
            {
                mGuiObjectManager = new GameObject(mGuiObjectManagerName);
                mGuiObjectManager.transform.parent = mRoot.transform;
                mGuiObjectManager.AddComponent<DialogueManagerBehaviour>();
            }
        }

        private static void createMenuObjectIfNotExist()
        {
            mMenuObject = GameObject.Find(mMenuObjectName);

            if (mMenuObject == null)
            {
                mMenuObject = new GameObject(mMenuObjectName);
                mMenuObject.transform.parent = mRoot.transform;
                mMenuObject.AddComponent<MenuBehaviour>();
            }
        }
    }
}

using UnityEngine;
using Vectrosity;

namespace CycleGotchi
{
	public class BikeLaneVisualBehaviour : MonoBehaviour
    {
        public BikeLaneBuilderBehaviour BikeLaneBuilder = null;
        public StreetGridBehaviour StreetGrid = null;
        public GameDirector GameDirector = null;

        public Color PreviewLineColor = Color.blue;
        public Color BuilderLineColor = Color.green;
        public Color BuilderLineColorLeft = Color.red;

        public Material PreviewLineMaterial = null;
        public Material BuilderLineMaterial = null;
        public Material BuilderLineLeftMaterial = null;
        public Material TutorialLineMaterial = null;

        public float PreviewLineMaterialTexScale = 1.0f;
        public float BuilderLineMaterialTexScale = 1.0f;

        public float PreviewLineThickness = 0.05f;
        public float BuilderLineThickness = 0.05f;

        private Vector3[] mTutorialPos = new Vector3[2];
        private Vector3[] mTutorialPosLeft = new Vector3[2];

        private Vector3[] mTutorialStartCirclePos = new Vector3[32];
        private Vector3[] mTutorialEndCirclePos = new Vector3[32];

        private Vector3[] mPreviewPos = new Vector3[2];

        private Vector3[] mBuilderPos = new Vector3[2];
        private Vector3[] mBuilderPosLeft = new Vector3[2];

        private VectorLine mTutorialLine;
        private VectorLine mTutorialLineLeft;
        private VectorLine mTutorialStartCircle;
        private VectorLine mTutorialEndCircle;

        private VectorLine mPreviewLine;

        private VectorLine mBuilderLine;
        private VectorLine mBuilderLineLeft;

        private bool mInputInProgressLastFrame = false;
        private bool mBuildInProgressLastFrame = false;
        private bool mTutorialInProgressLastFrame = false;

        
        private float mTutorialLineProgress = 0.0f;
        
        private float mTutorialLineLerpTime = 2.0f;
        private float mTutorialLineCurrentLerpTime = 0.0f;

        #region Unity Messages
        // Use this for initialization
        void Start()
        {
            VectorLine.SetCamera3D();
            if (BikeLaneBuilder == null)
                BikeLaneBuilder = GetComponent<BikeLaneBuilderBehaviour>();
        }

        // Update is called once per frame
        void Update()
        {
            bool inputInProgress = BikeLaneBuilder.InputIsInProgress;
            bool buildInProgress = BikeLaneBuilder.IsBuilding;
            bool tutorialInProgress = GameDirector.IsTutorialRound;

            if (inputInProgress)
            {
                clearLine(ref mPreviewLine);
                visualizeLinePreview();
            }

            if (mInputInProgressLastFrame && !inputInProgress)
            {
                clearLine(ref mPreviewLine);
            }

            if (buildInProgress)
            {
                clearLine(ref mBuilderLine);
                clearLine(ref mBuilderLineLeft);

                visualizeLineBike();

                //first line build by user - time to disable the tutorial!
                GameDirector.IsTutorialRound = false;
            }

            if (mBuildInProgressLastFrame && !buildInProgress)
            {
                clearLine(ref mBuilderLine);
                clearLine(ref mBuilderLineLeft);
            }

            if (GameDirector.IsTutorialRound && !inputInProgress && !buildInProgress)
            {
                clearLine(ref mTutorialLine);
                clearLine(ref mTutorialLineLeft);
                clearLine(ref mTutorialStartCircle);
                clearLine(ref mTutorialEndCircle);

                visualizeLineTutorial();

                mTutorialLineCurrentLerpTime += Time.deltaTime;
                if (mTutorialLineCurrentLerpTime > mTutorialLineLerpTime)
                    mTutorialLineCurrentLerpTime = mTutorialLineLerpTime;

                mTutorialLineProgress = mTutorialLineCurrentLerpTime / mTutorialLineLerpTime;

                if (mTutorialLineCurrentLerpTime == mTutorialLineLerpTime)
                    mTutorialLineCurrentLerpTime = 0.0f;
            }
            else
            {
                clearLine(ref mTutorialLine);
                clearLine(ref mTutorialLineLeft);
                clearLine(ref mTutorialStartCircle);
                clearLine(ref mTutorialEndCircle);
            }

            if (!GameDirector.IsTutorialRound && mTutorialInProgressLastFrame)
            {
                clearLine(ref mTutorialLine);
                clearLine(ref mTutorialLineLeft);
                clearLine(ref mTutorialStartCircle);
                clearLine(ref mTutorialEndCircle);
            }

            mInputInProgressLastFrame = inputInProgress;
            mBuildInProgressLastFrame = buildInProgress;
            mTutorialInProgressLastFrame = tutorialInProgress;
        }

        void OnDisable()
        {
            clearAllLines();
        }

        #endregion

        public void visualizeLineTutorial()
        {
            GameObject goStart = this.transform.Find("Tile B[0,3]").gameObject;
            GameObject goEnd = this.transform.Find("Tile C[4,3]").gameObject;

            if (goStart == null || goEnd == null)
                return;

            TileBehaviour tileStart = goStart.GetComponent<TileBehaviour>();
            TileBehaviour tileEnd = goEnd.GetComponent<TileBehaviour>();

            if (tileStart == null || tileEnd == null)
                return;

            float lineThickness = 0.01f;
            float circleRadius = 0.3f;

            Vector3 start = tileStart.getPositionOf(TileBehaviour.Corner.SOUTH_WEST) + new Vector3(circleRadius, 0.0f, 0.0f);
            Vector3 end = tileEnd.getPositionOf(TileBehaviour.Corner.SOUTH_EAST) - new Vector3(circleRadius, 0.0f, 0.0f);

            Vector3 head = start;
            head = start + (end - start) * mTutorialLineProgress;


            mTutorialPos[0] = start;
            mTutorialPos[1] = head;

            mTutorialLine = drawLine("TutorialLine", mTutorialPos, Color.green, lineThickness, TutorialLineMaterial, PreviewLineMaterialTexScale);


            mTutorialPosLeft[0] = head;
            mTutorialPosLeft[1] = end;

            mTutorialLineLeft = drawLine("TutorialLineLeft", mTutorialPosLeft, Color.green, lineThickness, PreviewLineMaterial, PreviewLineMaterialTexScale);


            Vector3 circleCenterStart = start - new Vector3(circleRadius + lineThickness / 2, 0.0f, 0.0f);
            Vector3 circleCenterEnd = end + new Vector3(circleRadius + lineThickness / 2, 0.0f, 0.0f);

            mTutorialStartCircle = drawCircle("TutorialStartCircle", mTutorialStartCirclePos, Color.green, lineThickness, circleCenterStart, StreetGrid.transform.up, circleRadius, mTutorialStartCirclePos.Length - 1);
            mTutorialEndCircle = drawCircle("TutorialStartCircle", mTutorialEndCirclePos, Color.green, lineThickness, circleCenterEnd, StreetGrid.transform.up, circleRadius, mTutorialEndCirclePos.Length - 1);
        }

        public void visualizeLinePreview()
        {
            Vector3 start;
            Vector3 end;

            if (BikeLaneBuilder.getCurrentStartPosition(out start))
            {
                mPreviewPos[0] = start;
            }

            if (BikeLaneBuilder.getCurrentEndPosition(out end))
            {
                mPreviewPos[1] = end;

                mPreviewLine = drawLine("PreviewLine", mPreviewPos, PreviewLineColor, PreviewLineThickness, PreviewLineMaterial, PreviewLineMaterialTexScale);
            }
        }

        public void visualizeLineBike()
        {
            if (BikeLaneBuilder.IsBuilding)
            {
                mBuilderPos[0] = BikeLaneBuilder.BuildStart;
                mBuilderPos[1] = BikeLaneBuilder.BuildCurrentHead;

                mBuilderPosLeft[0] = BikeLaneBuilder.BuildCurrentHead;
                mBuilderPosLeft[1] = BikeLaneBuilder.BuildTarget;

                mBuilderLine = drawLine("BuilderLine", mBuilderPos, BuilderLineColor, BuilderLineThickness, BuilderLineMaterial, BuilderLineMaterialTexScale);
                mBuilderLineLeft = drawLine("BuilderLineLeft", mBuilderPosLeft, BuilderLineColorLeft, BuilderLineThickness, BuilderLineLeftMaterial, BuilderLineMaterialTexScale);
            }
        }

        private VectorLine drawLine(string lineName, Vector3[] pos, Color color, float thickness, Material mat = null, float texScale = 1.0f)
        {
            VectorLine line = null;

            if (pos.Length >= 2)
            {
                float maxWidth = Mathf.Max(Screen.width, Screen.height);
                line = new VectorLine(lineName, pos, color, mat, thickness * maxWidth, LineType.Continuous, Joins.Weld);
                line.textureScale = texScale;

                line.Draw();
            }

            return line;
        }

        private VectorLine drawCircle(string name, Vector3[] pos, Color color, float thickness, Vector3 center, Vector3 up, float radius, int segments)
        {
            VectorLine line = drawLine(name, pos, color, thickness);

            line.MakeCircle(center, up, radius, segments);

            line.Draw();

            return line;
        }

        private void clearLine(ref VectorLine line)
        {
            if (line != null)
            {
                VectorLine.Destroy(ref line);
            }
        }

        private void clearAllLines()
        {
            clearLine(ref mPreviewLine);
            
            clearLine(ref mBuilderLine);
            clearLine(ref mBuilderLineLeft);

            clearLine(ref mTutorialLine);
            clearLine(ref mTutorialLineLeft);
            clearLine(ref mTutorialStartCircle);
            clearLine(ref mTutorialEndCircle);
        }
    }
}

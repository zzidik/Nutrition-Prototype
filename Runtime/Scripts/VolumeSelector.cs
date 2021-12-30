using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

namespace NutritionPrototype
{
    public class VolumeSelector : MonoBehaviour
    {
        /// <summary>
        /// The PlayerStation this selector belongs to.
        /// </summary>
        public PlayerStation playerStation;
        /// <summary>
        /// An ID number for this volume selector at a PlayerStation.
        /// Each PlayerStation will have more than one VolumeSelector.
        /// </summary>
        public int IDNum = 0;
        /// <summary>
        /// A list of volumes available to display.
        /// </summary>
        public List<Transform> volumes;
        /// <summary>
        /// The index of the volume being displayed.
        /// </summary>
        public int displayVolume = 0;

        public GameObject plate;
        private Material material;
        private Color startPlateColor;
        public Tween plate_Tween;
        private Color green = new Color(0f, 1f, 0f, 1f);
        private Color red = new Color(1f, 0f, 0f, 1f);

        private void Start()
        {
            material = plate.GetComponent<MeshRenderer>().materials[0];
            startPlateColor = material.color;
        }

        /// <summary>
        /// Sets the volume that the VolumeSelector displays and makes that volume active.
        /// Sets all other volumes inactive.
        /// </summary>
        /// <param name="dv">The index of the volume to be displayed.</param>
        public void SetDisplayVolume(int dv)
        {
            displayVolume = dv;
            int numVols = volumes.Count;
            for (int v = 0; v < numVols; v++)
            {
                if (v == displayVolume)
                {
                    volumes[v].gameObject.SetActive(true);
                }
                else
                {
                    volumes[v].gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Tells the PlayerStation that one of it's VolumeSelectors has been touched.
        /// </summary>
        public void VolumeTouched()
        {
            playerStation.VolumeTouched(displayVolume);
        }

        public void TweenPlateColor_Correct()
        {
            plate_Tween?.Kill();
            plate_Tween = material.DOColor(green, 0.5f);
        }

        public void TweenPlateColor_Incorrect()
        {
            plate_Tween?.Kill();
            plate_Tween = material.DOColor(red, 0.5f);
        }

        public void TweenPlateColor_Normal()
        {
            plate_Tween?.Kill();
            plate_Tween = material.DOColor(startPlateColor, 0.5f);
        }
    }
}

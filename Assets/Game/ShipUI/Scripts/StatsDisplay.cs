using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StatsDisplay : MonoBehaviour
{
    public GameObject statLabelText;
    public GameObject statBoostValueText;
    public float animationLength = 0.5f;

    // Holds the data used by a Lerp function to transition the displayed values
    private class AnimationData
    {
        public float initialValue;
        public float targetValue;
        public float time;

        public AnimationData(float x, float y, float z)
        {
            initialValue = x;
            targetValue = y;
            time = z;
        }
    }

    // This data is used to make the transitions between values smooth
    private Dictionary<ShipStat, AnimationData> animations = new Dictionary<ShipStat, AnimationData>();

    private Dictionary<ShipStat, float> boostedStats = new Dictionary<ShipStat, float>();
    public Dictionary<ShipStat, float> BoostedStats
    {
        get { return boostedStats; }
        set
        {
            boostedStats = new Dictionary<ShipStat, float>();
            string labelText = "";
            // Update the stats display to show the ship's stats
            foreach (ShipStat stat in value.Keys)
            {
                boostedStats.Add(stat, value[stat]);
                if (value[stat] > 0)
                {
                    labelText += stat.ToString() + "\n";
                }
            }
            statLabelText.GetComponent<Text>().text = labelText;
            redrawValues();
        }
    }

    void Update()
    {
        // Animate the transitions of stats as they change values
        if (animations.Count > 0)
        {
            // Mark stats which have finished for removal
            List<ShipStat> toRemove = new List<ShipStat>();
            foreach (ShipStat stat in animations.Keys)
            {
                // Step towards the target value
                boostedStats[stat] = Mathf.Lerp(
                    animations[stat].initialValue,
                    animations[stat].targetValue,
                    animations[stat].time / animationLength);

                animations[stat].time += Time.deltaTime;
                // Clean up stats which have finished transitioning
                if (animations[stat].time >= animationLength)
                {
                    boostedStats[stat] = animations[stat].targetValue;
                    toRemove.Add(stat);
                }
            }
            foreach (ShipStat stat in toRemove)
            {
                animations.Remove(stat);
            }
            redrawValues();
        }
    }

    // Change the displayed stat to the target value, animating the transition
    public void animateStatChange(ShipStat stat, float value)
    {
        AnimationData data = new AnimationData(boostedStats[stat], value, 0f);
        if (animations.ContainsKey(stat))
        {
            animations[stat] = data;
        } else
        {
            animations.Add(stat, data);
        }
    }

    // Update all of the stats, and animate the transitions
    public void animateAllStatChanges(Dictionary<ShipStat, float> newStats)
    {
        foreach (ShipStat stat in Enum.GetValues(typeof(ShipStat)))
        {
            animateStatChange(stat, newStats[stat]);
        }
    }

    // Rewrite all of the displayed values
    private void redrawValues()
    {
        string valueText = "";
        foreach (ShipStat stat in boostedStats.Keys)
        {
            if (boostedStats[stat] > 0)
            {
                valueText += string.Format("{0:#0.00}\n", boostedStats[stat]);
            }
        }
        statBoostValueText.GetComponent<Text>().text = valueText;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainMaskUtils
{

    public static float[,] GenerateCircularMask(int size, float maskMarginOffset)
    {

        float[,] maskValues = new float[size, size];

        for (int y = 0; y < size; y++)
        {

            for (int x = 0; x < size; x++)
            {
                float distanceX = Mathf.Abs(x - size * 0.5f);
                float distanceY = Mathf.Abs(y - size * 0.5f);
                float distance = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY); // circular mask

                float maxWidth = size * 0.5f - maskMarginOffset;
                float delta = distance / maxWidth;
                float gradient = delta * delta;

                maskValues[x, y] = Mathf.Max(0.0f, 1.0f - gradient);
            }

        }

        return maskValues;
    }

    public static float[,] GenerateSquareMask(int size, float maskMarginOffset)
    {

        float[,] maskValues = new float[size, size];

        for (int y = 0; y < size; y++)
        {

            for (int x = 0; x < size; x++)
            {
                float distanceX = Mathf.Abs(x - size * 0.5f);
                float distanceY = Mathf.Abs(y - size * 0.5f);
                float distance = Mathf.Max(distanceX, distanceY); // square mask

                float maxWidth = size * 0.5f - maskMarginOffset;
                float delta = distance / maxWidth;
                float gradient = delta * delta;

                maskValues[x, y] = Mathf.Max(0.0f, 1.0f - gradient);
            }

        }

        return maskValues;
    }

    public static float[,] ApplyCircularMask(float[,] originalData, int size, float maskMarginOffset)
    {
        float[,] maskData = TerrainMaskUtils.GenerateCircularMask(size, maskMarginOffset);
        return ApplyMask(originalData, maskData, size);
    }

    public static float[,] ApplySquareMask(float[,] originalData, int size, float maskMarginOffset)
    {
        float[,] maskData = TerrainMaskUtils.GenerateSquareMask(size, maskMarginOffset);
        return ApplyMask(originalData, maskData, size);
    }

    public static float[,] ApplyMask(float[,] originalData, float[,] maskData, int size)
    {
        float[,] maskedData = new float[size, size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                maskData[x, y] = originalData[x, y] * maskData[x, y];
            }
        }
        return maskData;
    }

}
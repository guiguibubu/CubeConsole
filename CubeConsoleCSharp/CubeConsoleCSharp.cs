using System;
using System.Linq;
using System.Threading;

namespace CubeConsoleCSharp
{
    internal class CubeConsoleCSharp
    {
        static float A, B, C;

        static int width = 160, height = 44;
        static float[] zBuffer = new float[160 * 44];
        static char[] buffer = new char[160 * 44];
        const char backgroundASCIICode = '.';
        static int distanceFromCam = 100;
        static float s_horizontalOffset;
        static float K1 = 40;

        static float incrementSpeed = 0.6f;

        static float x, y, z;
        static float ooz;
        static int xp, yp;
        static int idx;

        static float calculateX(int i, int j, int k)
        {
            return (float)(j * Math.Sin(A) * Math.Sin(B) * Math.Cos(C) - k * Math.Cos(A) * Math.Sin(B) * Math.Cos(C) +
                j * Math.Cos(A) * Math.Sin(C) + k * Math.Sin(A) * Math.Sin(C) + i * Math.Cos(B) * Math.Cos(C));
        }

        static float calculateY(int i, int j, int k)
        {
            return (float)(j * Math.Cos(A) * Math.Cos(C) + k * Math.Sin(A) * Math.Cos(C) -
                j * Math.Sin(A) * Math.Sin(B) * Math.Sin(C) + k * Math.Cos(A) * Math.Sin(B) * Math.Sin(C) -
                i * Math.Cos(B) * Math.Sin(C));
        }
        static float calculateZ(int i, int j, int k)
        {
            return (float)(k * Math.Cos(A) * Math.Cos(B) - j * Math.Sin(A) * Math.Cos(B) + i * Math.Sin(B));
        }

        static void calculateForSurface(float cubeX, float cubeY, float cubeZ, int ch)
        {
            x = calculateX((int)Math.Round(cubeX), (int)Math.Round(cubeY), (int)Math.Round(cubeZ));
            y = calculateY((int)Math.Round(cubeX), (int)Math.Round(cubeY), (int)Math.Round(cubeZ));
            z = calculateZ((int)Math.Round(cubeX), (int)Math.Round(cubeY), (int)Math.Round(cubeZ)) + distanceFromCam;

            ooz = 1 / z;

            xp = (int)(width / 2 + s_horizontalOffset + K1 * ooz * x * 2);
            yp = (int)(height / 2 + K1 * ooz * y);

            idx = xp + yp * width;
            if (idx >= 0 && idx < width * height)
            {
                if (ooz > zBuffer[idx])
                {
                    zBuffer[idx] = ooz;
                    buffer[idx] = (char)ch;
                }
            }
        }

        static void calculateCube(int cubeWidth, float horizontalOffset)
        {
            s_horizontalOffset = horizontalOffset;
            for (float cubeX = -cubeWidth; cubeX < cubeWidth; cubeX += incrementSpeed)
            {
                for (float cubeY = -cubeWidth; cubeY < cubeWidth;
                    cubeY += incrementSpeed)
                {
                    calculateForSurface(cubeX, cubeY, -cubeWidth, '@');
                    calculateForSurface(cubeWidth, cubeY, cubeX, '$');
                    calculateForSurface(-cubeWidth, cubeY, -cubeX, '~');
                    calculateForSurface(-cubeX, cubeY, cubeWidth, '#');
                    calculateForSurface(cubeX, -cubeWidth, -cubeY, ';');
                    calculateForSurface(cubeX, cubeWidth, cubeY, '+');
                }
            }
        }

        static void Main()
        {
            Console.Write("\x1b[2J");
            while (true)
            {
                buffer = Enumerable.Repeat(backgroundASCIICode, width * height).ToArray();
                zBuffer = Enumerable.Repeat(0f, width * height * 4).ToArray();
                // first cube
                {
                    int cubeWidth = 20;
                    int horizontalOffset = -2 * cubeWidth;
                    calculateCube(cubeWidth, horizontalOffset);
                }
                // second cube
                {
                    int cubeWidth = 10;
                    int horizontalOffset = 1 * cubeWidth;
                    calculateCube(cubeWidth, horizontalOffset);
                }
                // third cube
                {
                    int cubeWidth = 5;
                    int horizontalOffset = 8 * cubeWidth;
                    calculateCube(cubeWidth, horizontalOffset);
                }
                Console.Write("\x1b[H");
                for (int k = 0; k < width * height; k++)
                {
                    Console.Write(k % width != 0 ? buffer[k] : (char)10);
                }

                A += 0.05f;
                B += 0.05f;
                C += 0.01f;
                Thread.Sleep(12);
            }
        }
    }
}
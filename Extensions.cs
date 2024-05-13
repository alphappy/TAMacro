using UnityEngine;
using System.Linq;

namespace alphappy.TAMacro
{
    public static class Extensions
    {
        public static Player.InputPackage WithDownDiagonals(this Player.InputPackage package)
        {
            if (ModManager.MMF)
            {
                if (package.analogueDir.y < -0.05f || package.y < 0)
                {
                    if (package.analogueDir.x < -0.05f || package.x < 0)
                    {
                        package.downDiagonal = -1;
                    }
                    else if (package.analogueDir.x > 0.05f || package.x > 0)
                    {
                        package.downDiagonal = 1;
                    }
                }
            }
            else if (package.analogueDir.y < -0.05f)
            {
                if (package.analogueDir.x < -0.05f)
                {
                    package.downDiagonal = -1;
                }
                else if (package.analogueDir.x > 0.05f)
                {
                    package.downDiagonal = 1;
                }
            }
            return package;
        }

        public static string AsString(this Player.InputPackage package, string filler = "")
        {
            return $"{(package.x == -1 ? "L" : (package.x == 1 ? "R" : filler))}{(package.y == -1 ? "D" : (package.y == 1 ? "U" : (package.analogueDir.y == -0.06f ? "d" : filler)))}{(package.jmp ? "J" : filler)}{(package.pckp ? "G" : filler)}{(package.thrw ? "T" : filler)}{(package.mp ? "M" : filler)}";
        }

        public static bool EqualTo(this Player.InputPackage self, Player.InputPackage other)
        {
            return self.analogueDir == other.analogueDir && self.x == other.x && self.y == other.y && self.jmp == other.jmp && self.pckp == other.pckp && self.thrw == other.thrw && self.mp == other.mp;
        }

        public static Rect BoundingBox(this FSprite self)
        {
            return new Rect(
                self.GetPosition() + self.container.GetPosition() - new Vector2(self.width, self.height) / 2f, 
                new Vector2(self.width, self.height)
                );
        }

        public static bool AmIBeingHovered(this FSprite self) => self.BoundingBox().Contains(Input.mousePosition);

        public static void SetBounds(this FSprite self, Rect bounds)
        {
            self.SetPosition(bounds.center);
            self.width = bounds.width;
            self.height = bounds.height;
        }

        public static Rect Shifted(this Rect self, Vector2 shift) => new(self.position + shift, self.size);

        public static Rect GetFixedWidthBounds(this FLabel self)
        {
            var lines = self.text.Trim().Split('\n');
            var width = lines.Max(line => line.Length) * 8;
            var height = lines.Length * self.FontLineHeight;
            return new(self.x - width / 2, self.y - height / 2, width, height);
        }
        public static void RootTop(this FLabel self, float yRoot)
        {
            self.y = yRoot - self.GetFixedWidthBounds().height / 2;
        }
        public static void SetTextAndRootTop(this FLabel self, string text, float yRoot)
        {
            var lines = text.Trim().Split('\n');
            self.text = text;
            var height = lines.Length * self.FontLineHeight;
            self.y = yRoot - height / 2;
        }

        public static Rect Resized(this Rect self, Vector2 resize) => new(self.position, self.size + resize);
        public static Rect Resized(this Rect self, float w, float h) => new(self.x, self.y, self.width + w, self.height + h);
    }
}

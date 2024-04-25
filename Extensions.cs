using UnityEngine;

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
            return $"{(package.x == -1 ? "L" : (package.x == 1 ? "R" : filler))}{(package.y == -1 ? "D" : (package.y == 1 ? "U" : filler))}{(package.jmp ? "J" : filler)}{(package.pckp ? "G" : filler)}{(package.thrw ? "T" : filler)}{(package.mp ? "M" : filler)}";
        }

        public static bool EqualTo(this Player.InputPackage self, Player.InputPackage other)
        {
            return self.x == other.x && self.y == other.y && self.jmp == other.jmp && self.pckp == other.pckp && self.thrw == other.thrw && self.mp == other.mp;
        }

        public static Rect BoundingBox(this FSprite self)
        {
            return new Rect(
                self.GetPosition() + self.container.GetPosition() - new Vector2(self.width, self.height) / 2f, 
                new Vector2(self.width, self.height)
                );
        }

        public static bool AmIBeingHovered(this FSprite self) => self.BoundingBox().Contains(Input.mousePosition);
    }
}

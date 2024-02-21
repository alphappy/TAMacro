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
    }
}

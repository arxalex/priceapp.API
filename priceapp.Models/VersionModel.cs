namespace priceapp.Models;

public class VersionModel
{
    public int Id { get; set; }
    public int Version { get; set; }
    public int Major { get; set; }
    public int Minor { get; set; }
    public bool IsMinVer { get; set; }

    public static bool operator <(VersionModel left, VersionModel right)
    {
        if (left.Version < right.Version)
        {
            return true;
        }
        if (left.Version > right.Version)
        {
            return false;
        }

        if (left.Major < right.Major)
        {
            return true;
        }
        if (left.Major > right.Major)
        {
            return false;
        }

        if (left.Minor < right.Minor)
        {
            return true;
        }

        return false;
    }

    public static bool operator >(VersionModel left, VersionModel right)
    {
        if (left.Version > right.Version)
        {
            return true;
        }
        if (left.Version < right.Version)
        {
            return false;
        }

        if (left.Major > right.Major)
        {
            return true;
        }
        if (left.Major < right.Major)
        {
            return false;
        }

        if (left.Minor > right.Minor)
        {
            return true;
        }

        return false;
    }
    
    public static bool operator >=(VersionModel left, VersionModel right)
    {
        if (left.Version > right.Version)
        {
            return true;
        }
        if (left.Version < right.Version)
        {
            return false;
        }

        if (left.Major > right.Major)
        {
            return true;
        }
        if (left.Major < right.Major)
        {
            return false;
        }

        if (left.Minor >= right.Minor)
        {
            return true;
        }

        return false;
    }

    public static bool operator <=(VersionModel left, VersionModel right)
    {
        if (left.Version < right.Version)
        {
            return true;
        }
        if (left.Version > right.Version)
        {
            return false;
        }

        if (left.Major < right.Major)
        {
            return true;
        }
        if (left.Major > right.Major)
        {
            return false;
        }

        if (left.Minor <= right.Minor)
        {
            return true;
        }

        return false;
    }
}
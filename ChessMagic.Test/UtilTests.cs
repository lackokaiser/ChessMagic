using ChessMagic.Util;

namespace ChessMagic.Test;

public class UtilTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void PositionInitializationTest()
    {
        Position pos = new Position(1, 2);
        Assert.That(pos.X, Is.EqualTo(1));
        Assert.That(pos.Y, Is.EqualTo(2));

        pos = new Position(0, 8);
        Assert.That(pos.X, Is.EqualTo(0));
        Assert.That(pos.Y, Is.EqualTo(8));
    }

    [Test]
    public void PositionOffsetTest()
    {
        Position pos = new Position(1, 2);

        Position off = pos.Offset(1, 2);
        
        Assert.That(off.X, Is.EqualTo(2));
        Assert.That(off.Y, Is.EqualTo(4));

        off = off.Offset(-1, -1);

        Assert.That(off.X, Is.EqualTo(1));
        Assert.That(off.Y, Is.EqualTo(3));
    }

    [Test]
    public void PositionEqualsTest()
    {
        Position pos = new Position(1, 2);
        Position otherPos = new Position(2, 1);
        
        Assert.IsFalse(pos.Equals(otherPos));
        Assert.IsFalse(otherPos.Equals(pos));
        
        Position match = new Position(1, 2);
        
        Assert.IsTrue(pos.Equals(match));
        Assert.IsTrue(match.Equals(pos));
        
        Assert.IsFalse(otherPos.Equals(match));

    }

    [Test]
    public void PositionInvalidTest()
    {
        Position pos = Position.Invalid;
        
        Assert.IsTrue(pos.IsInvalid());

        pos = new Position(1, 1);
        
        Assert.IsFalse(pos.IsInvalid());
    }
}
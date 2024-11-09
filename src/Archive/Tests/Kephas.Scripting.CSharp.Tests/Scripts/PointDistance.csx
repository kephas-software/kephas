public record Point(int x, int y);

var first = new Point(x1, y1);
var second = new Point(x2, y2);

return (first.x - second.x) * (first.x - second.x) + (first.y - second.y) * (first.y - second.y); 
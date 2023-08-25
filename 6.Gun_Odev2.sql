SELECT Products.ProductName AS [�r�n Ad�], SUM([Order Details].UnitPrice * [Order Details].Quantity) as [Kazan�lan Toplam Miktar]
FROM Products
INNER JOIN [Order Details] on Products.ProductID = [Order Details].ProductID
INNER JOIN Orders on [Order Details].OrderID = Orders.OrderID
GROUP BY Products.ProductName
SELECT Products.ProductName AS [Ürün Adý], SUM([Order Details].UnitPrice * [Order Details].Quantity) as [Kazanýlan Toplam Miktar]
FROM Products
INNER JOIN [Order Details] on Products.ProductID = [Order Details].ProductID
INNER JOIN Orders on [Order Details].OrderID = Orders.OrderID
GROUP BY Products.ProductName
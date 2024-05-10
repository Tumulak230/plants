SELECT U.username, U.password, R.roleName, P.ProductName, C.CategoryName, P.ProductImg, P.description, P.price
FROM [User] U
INNER JOIN UserRole UR ON U.id = UR.userId
INNER JOIN Role R ON UR.roleId = R.id
INNER JOIN Products P ON U.id = P.UserId
INNER JOIN Category C ON P.CategoryId = C.id

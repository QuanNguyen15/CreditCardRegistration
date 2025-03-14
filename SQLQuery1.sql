Use CreditCardRegistrationDB
GO

select * from Users
select * from CreditCards
where Status = 'Active'
select * from CardTypes

DELETE FROM Users
WHERE USERID = 10


select Users.UserID, Users.Phone, Users.FullName, Users.Username, CreditCards.Status,
CreditCards.CardNumber, CreditCards.CVV, CreitCards.CreatedDate, CardTypes.CardTypeName
from CreditCards
INNER JOIN Users ON Users.UserID = CreditCards.UserID
INNER JOIN CardTypes ON CreditCards.CardTypeID = CardTypes.CardTypeID

INSERT INTO CardTypes (CardTypeName, Benefits)
VALUES
('Visa', '5% cashback on shopping'),
('Mastercard', 'Free airport lounge access'),
('Amex', 'Exclusive travel benefits');


SELECT DISTINCT CardTypeName, Benefits
INTO #TempCardTypes
FROM CardTypes;


DELETE FROM CardTypes;

INSERT INTO CardTypes (CardTypeName, Benefits)
SELECT CardTypeName, Benefits FROM #TempCardTypes;

DROP TABLE #TempCardTypes;



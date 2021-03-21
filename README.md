# -MemberManagementSystem

Possible interview question!

Please implement a Member Management System. We need to save and manage information about our 
members (name and address). Each member can have one or more accounts where he or she can collect 
points (e.g.: for one international flight with Lufthansa the user gets 150 points) or redeem points from 
(e.g.: 100 points from his/her account in exchange for a free coffee). Points are stored as a balance on 
each account. Each account has a name identifying the company from which the points were collected 
and status telling if the account is active or inactive. Points cannot be redeemed from an inactive or empty 
account.
The system could cover the following use cases:
1. user creates a new member
2. user creates a new account for a defined member
3. member collects points to an existing account
4. member redeems points from an existing account
5. user can initially import existing members in a JSON format (example is attached)
6. user can export all members based on filter criteria (e.g.: export all members that have at least 20 
points on an inactive account)
7. Dump generaded log (generate logfiles)

There is no need to implement the GUI. You can implement the assignment as a console application, 
desktop application or Web API. It is up to you.

If you have any suggestion or question please contact me here: info@herrmannsven.de

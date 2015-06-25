# TDD-to-Mock-or-not-to-Mock
A scenario where I got lazy and used a database in my tests.

I used a database to test I could store only duplicates in a list. Later on, I decided to find a new way to store duplicates.

But by that time, I had already done some production code which altered my database. Therefore the hard coded ids didnt all exist anymore.

Therefore my test failed!!!

I have put my old test and new test where the new test uses mocking and can never break unless I change the hard coded list data.

# Summary

This is a repo containing of Facebook bots. I've setup a base service and worker class that could be easily reused to create a new bot that posts based on CRON rules.

# CyanideAndHappinessBot

This bot fetches a comic from the [Cyanide&amp;Happiness random comic generator](https://explosm.net/rcg) and posts it on the [Cyanide&Happiness Bot 5000](https://www.facebook.com/cyanideandhappinessbot5000)'s Facebook page.

The bot uses Selenium WebDriver to wait for the comic to fully load, scrapes the three images, builds a new one in memory and posts it.

# RandomPersonBot

This is a bot that fetches an image from [This person does not exist](https://thispersondoesnotexist.com/)'s website and posts it to the [Hourly picture of a person that does NOT exist](https://www.facebook.com/Hourly-picture-of-a-person-that-does-NOT-exist-104894912066359)'s Facebook page

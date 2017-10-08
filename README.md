# Janitra

Janitra is a proposed CI-like system for running and recording tests against Citra.
It manages the central repository of tests, and stores and presents the results that are submitted.

The goal is that whenever a commit is made to Citra master, or a PR is created, these tests will be automatically ran against the resulting binary, to help us to catch regressions and find places where emulation results have changed.

## Tests

### Hardware Test Roms
These are roms created by Citra Developers that test small specific parts of the emulator.
The results they produce can be defined as "Correct" or "Incorrect".

These can be uploaded and stored in Janitra.
JanitraBot will automatically download and run these and upload the output. If it matches a known good output it will be recorded as Correct.

### Commercial Roms
Commercial Roms are the Roms that are shipped on Cartridges or downloaded from the EShop.
We run these roms in Citra to find out how well Citra is performing in both accuracy and speed.
They can help us catch regressions that the more targeted Hardware Tests may miss.

Commercial Roms are not stored in Janitra, they must be supplied by the user running JanitraBot.
The 'Movie Replay' (Game Inputs) are stored in Janitra, these define what happens in the test (What buttons are pressed and when).
Providing the same inputs should always produce the same output, unless we have changed behaviour.

## [JanitraBot](https://github.com/danzel/Janitra.Bot)

JanitraBot is a program that you can run locally to help run the tests stored in Janitra.
If you own one of the games that has tests, you can run JanitraBot to submit accuracy results for it.
It is not really ready for public usage yet, soon hopefully :)

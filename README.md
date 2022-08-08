
# Soul Link Pair

## Introduction

Pokémon is a video game developed for Nintendo Systems since 1996. Many people within the community have created different challenge runs, such as the Monotype, Single-Pokémon, Nuzlocke, Ironman, and Nuzlocke challenges. One such challenge, a variant of the Nuzlocke challenge, is called a Soul Link. In a Soul Link, you and a friend play through the same game simultaneously. The rules are as follows:
1. You can only catch the first Pokémon you encounter on a route.
2. When you and your friend catch a Pokémon, the two are "Soul Linked." If one faints, it and the Pokémon it is soul linked with must be released.
3. Nickname every Pokémon you catch.
4. The primary types of all Pokémon currently on the team must be unique. For example, if one player has a primary water-type, that must be the only primary water-type on either team at that moment.

Rule #4 can make the possible teams a logistical nightmare to track. This program can aid in finding the best possible combinations with minimal effort from the players' side. In a CSV file, have the following columns (with their headers) in order:
* Location
* Player 1's Pokémon
* That Pokémon's type
* Player 2's Pokémon
* That Pokémon's type

For example, you might have the first three encounters of Kanto as shown below
|Location|Pokémon 1|Type 1|Pokémon 2|Type 2|Comment|
|--|--|--|--|--|--|
|Pallet Town|Charmander|Fire|Squirtle|Water|
|Route 1|Pidgey|Normal|Rattata|Normal|
|Route 2|Rattata|Normal|Caterpie|Bug|Dead

In the above, Pidgey and Rattata can't be used because they have the same type, and while the Route 2 encounters were valid, they fainted at some point and cannot be used in pairings anymore.

The program will assign each pair a score based on their stat totals then offer the best team for either Player 1, Player 2, both, or teams containing specific Pokémon.

## Requirements

No external libraries are needed to run the program.

## Installation

Download this code repository by using git:

`git clone https://github.com/Bassel-T/SoulLinkPair.git`

## Support

For help from the developer, please leave an issue in the issue tracker.

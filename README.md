# The Omen Den L.L.C Presents: Crows Against Humility

## A blazor based Cards Against Humility Client 
### Not affiliated with, endorsed by, or associated with the official Cards Against Humanity people/company. 
This is purely a *passion* project.

## We aim to create an environment where content creators can play this game online and with little to no fear of breaking their medium's TOS by enabling custom filters on cards, decks, and other terms. 
   - By Having these filters in place, we can increase reliability, and safety. 
   - We also can reduce potentially triggering cards that might make audiences uncomfortable. 

## We also aim to provide a way to integrate the game with Twitch Chats, and Discord communities
   - Allowing for more "secure" game creation - by linking participants to known twitch handles, and discord names with ids. 
     - We accomplish this by directly interfacing with both the Twitch API and Discord API. 
     - This should in turn help reduce the risk for unknown and bad faith actors who aim to "snipe" games while a content creator is in the middle of production.
     - This should also enable certain chat interactions that otherwise aren't present (for example, forcing a content creator to avoid playing cards that contain a certain word for a specified time limit via Twitch Channel Point Redeem)
     - We can also save game records, and provide recovery invites should a player be disconnected from their Session.
   - As mentioned above there can be twitch channel point redeems, but also we can have an audit log of points won, games won, and who had what card at the end of a round, for simplicty and accessibility. 
     - Of course, we aim to help prevent "spoilers" - but our methodology for this is still in the hypothesis stage

## This service will be hosted on Azure for a main operating environment [Crows Against Humility](https://crowsagainsthumility.app)
   - This will be of course operated by The Omen Den
   - We will also provide support as needed
   - And create regular updates on a specified quarterly cadence
   - We have secured a domain, as well as an EV SSL Certificate
   - We also work within the Microsoft Identity Platform for authentication and authorization.

### Credits and Sources for cards, and components:
1. [Cards Against Humanity](https://www.cardsagainsthumanity.com/) for the actual, original game (still, no affiliation, endorsement, or association with The Omen Den in any way).
2. [JSON Against Humanity](https://www.crhallberg.com/cah/) - for the JSON based cards we use.
3. [Blazorise](https://blazorise.com) for the blazor components (and our continued partnership)
4. [Fluxor](https://github.com/mrpmorris/Fluxor) For powering our State Managed Game
5. [The Omen Den](https://theomenden.com) 

#### The Omen Den appreciates your support for this project
- Come check out main Development streams: [Alu The Crow on Twitch](https://twitch.tv/aluthecrow)

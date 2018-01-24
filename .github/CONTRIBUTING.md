# Contributing
This document contains rules and guidelines on how to contribute to the project effectively. These rules are used to keep code and documentation consistent, and prevent future headaches.

## Installation
Place holder

## Development
Place holder

## Testing
Place holder

## Coding Style
Check out [CODINGSTYLE.md](https://github.com/bcitdatacomm/game/blob/master/.github/CODINGSTYLE.md)

## Commit Messages
Good commit messages strike a balance between short and descriptive. Try to explain exactly what was changed in the commit message, and if applicable, why it was changed. Each commit should be small and generally should only contain a single change or set of closely related or duplicate changes. 

## Workflow
Team feature branches should follow the naming scheme `feature-[teamname]-[featurename]`. Try to follow this name scheme so teams know which branches belong to them and do not accidentally delete your branches. Feel free to use your team branches in whatever way works best for you, just keep in mind that each team must follow the rules explained below and in Pull Requests.

When your feature is ready to be merged on to your teams staging branch, speak with your team lead and then create a Pull Request for review. Make sure you first rebase `develop` to your teams staging branch, and then rebase from your teams staging branch to your personal feature branch before creating the Pull Request.

When your team is ready to make a Pull Request onto develop, rebase from `develop` onto your teams staging branch before creating a Pull Request or it will be automatically rejected. This is to make sure your code is working with the most recent version of `develop`.

## Pull Requests
Pull requests onto the `develop` branch should only be made by admins or team leads. If you feel that your team has reached a point where all of your features are stable and ready to be merged into the main staging branch, contact your team lead and inform them that you want to create a Pull Request. 

For internal pull requests within your team, discuss with your team members the guidelines that you plan to follow.
# EWU-CSCD371-2026-Fall

## About the Essential C# Class

### Instructors

[Mark Michaelis](https://github.com/MarkMichaelis), [Benjamin Michaelis](https://github.com/BenjaminMichaelis), and [Kevin Bost](https://github.com/Keboo)

For any general questions, we suggest asking in [EWU-CSCD371-2026-Fall Microsoft Teams](https://teams.microsoft.com/l/team/19%3AAKcgFFJRsiO0qNS2E2rdFiyGdMTPWJ1OlMhvFK3PGUw1%40thread.tacv2/conversations?groupId=b160d26b-6ca1-4cde-83e3-dbcba8dc8b80&tenantId=37321907-14a5-4390-987d-ec0c66c655cd). This is a great place to collaborate and ask questions of both each other and the instructors. Though you can message people directly, we encourage open discussion on the meeting chat so that others can benefit as well.

You may send an e-mail with questions that are specific to you to <EWU-Instructors@IntelliTect.com>. Examples include:

* a notification that you are bringing pizza for everyone or
* you are dropping the class

#### Online Class

Class will NOT generally be available online.

#### GitHub Information

Please fill out this [Essential C# 2026 Fall Information – Fill out form](https://forms.cloud.microsoft/r/uGzgTuXyi2) with your GitHub information. This is used by us so we can appropriately grade assignments.

#### Book

* [Essential C# Book Online](https://EssentialCSharp.com)
* Essential C# 12.0 (8th Edition), ISBN-13: 978-0138219512

#### General Information

* [**Grading System**](./Docs/Homework-Grading.md)
* **Office Hours**: Office hours are available before class on Tuesdays and Thursdays starting at 1 PM.  To ensure that an instructor will be available, please schedule an appointment 24-hours beforehand by sending a meeting request email to <EWU-Instructors@IntelliTect.com>.  Alternative times may be available after class upon request.
* **Code for the Website**: <https://github.com/IntelliTect/EssentialCSharp.Web>
* **Code for the Book Examples**: <https://github.com/IntelliTect/EssentialCSharp>
* [Teams Chat Room](https://teams.microsoft.com/l/team/19%3AAKcgFFJRsiO0qNS2E2rdFiyGdMTPWJ1OlMhvFK3PGUw1%40thread.tacv2/conversations?groupId=b160d26b-6ca1-4cde-83e3-dbcba8dc8b80&tenantId=37321907-14a5-4390-987d-ec0c66c655cd)

#### Computer Setup

##### Required

* [.NET latest](https://dotnet.microsoft.com/download)

Most of the coursework is cross-platform. Our recommended IDEs are [JetBrains Rider](https://www.jetbrains.com/rider/), [Visual Studio Code](https://code.visualstudio.com/), and [Visual Studio 2026](https://visualstudio.microsoft.com/) — use whichever you're most comfortable with (latest release of any of the three is fine).

###### Recommended

* [JetBrains Rider](https://www.jetbrains.com/rider/) A cross-platform full C# IDE from JetBrains.
* [Visual Studio Code](https://code.visualstudio.com/) A lightweight, cross-platform editor with excellent C# support via extensions.
* [Visual Studio 2026](https://visualstudio.microsoft.com/) The community edition is fine, though we believe most students should have access to higher SKUs with EWU's MSDN (this assumption may be wrong). Lab computers should have Visual Studio Professional (or higher) already installed. When installing, select the **“ASP.NET and web development” workload**.
* [GitHub Student Developer Pack](https://education.github.com/students)
  There are lots of great development tools and resources. The JetBrains ReSharper (a plugin for Visual Studio) is a great tool for helping you write better code.
* [GitKraken](https://gitkraken.cello.so/tl7bYaRLgzT)
  Though you can do all of the git interaction from within your IDE or on the command line, GitKraken is free for open-source work. It also provides a nice graphical version of the commit history so you can see how various commits and branches relate.

###### Optional

* [Visual Studio Live Share](https://visualstudio.microsoft.com/services/live-share/) This lets you easily collaborate with other people on a shared set of code.
* [GitExtensions](https://gitextensions.github.io/), [GitHub Desktop](https://desktop.github.com/), (or any other git tool): There are lots of options out there for working with git. If there is a tool you like, use it!

#### Class Video

We will try to record the class so that students can review it at a later time. Recordings will be available under Files in the [Teams channel](https://teams.microsoft.com/l/team/19%3AAKcgFFJRsiO0qNS2E2rdFiyGdMTPWJ1OlMhvFK3PGUw1%40thread.tacv2/conversations?groupId=b160d26b-6ca1-4cde-83e3-dbcba8dc8b80&tenantId=37321907-14a5-4390-987d-ec0c66c655cd) (SharePoint link to be added once the 2026 Recordings folder is created).

Please note:

* **Do not expect or rely on video recordings.**  We are not making any commitment to having video recordings of the class.
* Please keep all videos ***confidential***. These videos are for people enrolled in this class.  No videos or parts of videos may be copied/distributed/shared.

#### [Calendar](./Docs/Schedule.md)

## Assignment CI for maintainers

Assignment branches use `pull_request` to call the public reusable workflow in
`.github/workflows/build-and-test.yml@main`. Builds run in this repository with
read-only permissions, including PRs from student forks. Students do not need to
enable Actions on their forks; maintainers must still approve runs when required
by the repository's external-contributor policy.

The build publishes its coverage summary as an artifact and job summary, including
available coverage when tests fail. `Report PR results`, installed on `main`,
responds to the completed run and posts a sticky PR comment with the outcome,
commit, run link, and available coverage. Build failures without coverage still
receive a status comment. Reporting errors are visible in the reporter's logs.
The reporter alone can write comments: it validates the PR association and result
freshness, and never checks out or executes student code. These results provide
feedback, not tamper-proof grading.

When releasing an assignment from the private instructor repository:

1. Keep its workflow name `Build and Test .NET`, use `pull_request`, and grant only
   `contents: read`. Preserve that assignment's existing workflow inputs.
2. Call
   `IntelliTect-Samples/EWU-CSCD371-2026-Fall/.github/workflows/build-and-test.yml@main`,
   not the private instructor workflow. Instructor branches are release templates;
   running CI in the instructor repository is not required.
3. Publish shared build/reporting changes to public `main` before releasing callers.
   Do not replace public `main` with the old instructor workflow implementation.
4. For an existing fork or PR, sync the updated public assignment branch into the
   student's branch and resolve its workflow to the new caller. Updating `main`
   alone does not trigger a run. Push the synced branch, or reopen the PR after its
   workflow is corrected, to generate a new PR event.

Do not use `pull_request_target` or enable unsafe PR checkout for student builds.

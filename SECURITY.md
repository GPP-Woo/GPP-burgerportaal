# Security Policy

The GPP-burgerportaal development team takes security of our application seriously.
We very much appreciate your help in identifying and addressing any security vulnerabilities.
While we have automated processes in place to help preventing security vulnerabilities in the application as much as possible,
we realize that this will never give a 100% guarantee.

## Reporting a vulnerability

If you believe you have found a security vulnerability in our code base, please report it to us through coordinated disclosure.

Please do not report security vulnerabilities through public GitHub issues, discussions, or pull requests.

Instead, please use one of the following channels:

- **Preferred**: [GitHub Security Advisories](https://github.com/GPP-Woo/GPP-burgerportaal/security/advisories/new) — opens a private report directly with the team.
- **Email**: [the maintainers](mailto:servicedesk@icatt-info.nl).

Please include as much of the information listed below as you can to help us better understand and resolve the issue:

- The type of issue (e.g., buffer overflow, SQL injection, or cross-site scripting)
- Full paths of source file(s) related to the manifestation of the issue
- The location of the affected source code (tag/branch/commit or direct URL)
- Any special configuration required to reproduce the issue
- Step-by-step instructions to reproduce the issue
- Proof-of-concept or exploit code (if possible)
- Impact of the issue, including how an attacker might exploit the issue

This information will help us triage your report more quickly.

## Timeline of the process

The product's community support is provided by [ICATT]. The community support 
team is responsible for the handling of security issues.

1. The recipients of the report first validate if there is indeed a (possible) 
   issue.

2. After validation, we confirm that we received the report and if it is indeed
   a valid issue.

3. We have a private Github repository accessible only to the community support 
   team. In this repository, an issue is created for the vulnerability where 
   the impact and possible solutions are discussed.

4. The next step is to create a (draft) Github security advisory, which is only 
   visible to the repository administrators and community support team. 
   Severity and impact will be established here.

5. If appropriate, we request a [CVE identifier][CVE_identifier] from Github.

6. A patch is implemented, reviewed and tested in a private fork.

7. When the fix is tested and release coordination is done, the fix is merged 
   into the primary repository. The security advisory and release are 
   published. All managed instances should be updated.

8. The release and security vulnerability are communicated to the community. 
   This includes an announcement on the most relevant public community 
   channels.

## CVE assignment

For confirmed vulnerabilities, we will request a CVE identifier via GitHub's CVE Numbering Authority (CNA) programme. The CVE will be published together with the GitHub Security Advisory at the time of public disclosure.

## Our commitment to reporters

- We will not take legal action against researchers who follow responsible disclosure practices.
- We will keep you informed throughout the process.
- We will credit you in the published advisory unless you prefer to remain anonymous.

[CVE_identifier]: https://www.cve.org/
[ICATT]: https://www.icatt.nl

# Brutus

A tool to help generate password cracking masks for tools like hashcat.
The idea is to feed it a list of known good passwords for an environment
and it will analyze those passwords to try to generate a mask that will
attempt to maximize finds and minimize the brute force space.

The basic assumption is that in an envrionment, password policies tend to
drive similar behavior among people that are not using randomly generated
passwords.

We had mixed success in our experiments and decided that it would just be
simplier to mandate longer passwords to help mitigate the thread of
brute force attacks on the any dumped hashes.

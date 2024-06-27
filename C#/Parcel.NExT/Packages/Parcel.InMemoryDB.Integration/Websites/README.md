# Parcel InMemoryDB Database Administration Website

Designed for LAN sharing of database data, there is no account administration.

## TODO

Make sure no matter how simple it is, it looks complete, and professional and trustworthy!

- [x] Refine overall layout
- [x] VS Code or other desktop-software-like interface style
- [ ] Show table creation scheme (when click on table name show as preview)
- [ ] Table click preview first 100 lines (also update example query)
- [x] Query "executing" spinner status indicator; 
    - [x] Disable `Submit` when executing; 
    - [ ] Enable "Stop" when executing.
        - [ ] Implement actual stop logic.
- [x] Download result as CSV.
- [ ] Avoid any obvious "incompleteness/novice" problems: 1) Weird color, 2) Unreadable fonts, 3) Lack of proper margin and spacing, 4) No responsive contexts when executing operations. 5) No spending at least 48 hours just on drsign, styling, and making it looking good.
- [x] Query execution time measurement and reporting
- [ ] Fix .SVG issue

Won't do:

* No plotting support
* No table creation GUI
* (Negotiable) No cell editing, row creating/editing (like SQLite DB browser)
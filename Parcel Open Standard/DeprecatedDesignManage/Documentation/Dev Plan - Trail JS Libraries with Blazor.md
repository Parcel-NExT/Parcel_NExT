Web Assembly  + JS Frame/Panel - like Photoshop online, a self-contained single-page web-app.

# Remark

Non-react solutions (e.g. vanilla JsPanel) requires serious hacking because it can't automatically update the front-end. (jsframe seems to work with React?)

# Component Candidates

Try to make things simple and clean, don't use advanced components unless we have to.

* JsPanel4 (Looks not intended for window-environment but more debugging or QA use; But it actually is good): https://github.com/Flyer53/jsPanel4, https://github.com/priteshjha4u/react-jspanel4
* JSFrame (Less Mature): https://github.com/riversun/JSFrame.js/, https://dev.to/riversun/introduction-of-floating-window-library-jsframejs-14ho
* Telerik UI (Blazor native; super expensive; Not sure whether it works at all): https://docs.telerik.com/blazor-ui/components/window/overview
* Vue (Too low-level): https://vuejs.org/examples/#todomvc
* UIKit (Too Low-level): https://github.com/uikit/uikit, https://github.com/zebzhao/UION
* JsonForm (Optional Component): https://jsonforms.io/
* SpekUI (Least Stars): https://github.com/spckio/spck-ui
* jsgrids.statico.io (Survey): https://jsgrids.statico.io/
* EditableGrid (Good but seemingly same as jQuery Grid; Also, official website sucks): http://www.editablegrid.net/en/
* In-memory SQL Engine (for aggregating data) (Major feature!): https://docs.microsoft.com/en-us/ef/core/providers/in-memory/?tabs=dotnet-core-cli, https://nmemory.net/, https://docs.microsoft.com/en-us/dotnet/standard/data/sqlite/in-memory-databases, https://blog.learningtree.com/net-core-in-memory-database/

# JQuery

* Table/Grid Editor (jQuery): http://js-grid.com/demos/

# Summary

Won't work will non-Blazor native components; Will not purchase; Will develop from ground up.
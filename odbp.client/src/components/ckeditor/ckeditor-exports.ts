// we export these from here so we can combine asynchronous imports with tree shaking, because
// we don't want to import the entire ckeditor (huge) and
// we don't want to import it all the time (rarely used wtihin the application)
// so we cant implement it as documented with .use(CKEditor) in the main.ts
// we want to import it asynchronously so the code gets split into a seperate file, downloaded on demand.
// therefore we first import the bits we need in here and do an async import of this file wherever we need the editor
export { Ckeditor } from "@ckeditor/ckeditor5-vue";

export { ClassicEditor, Heading, Link, List, Paragraph, type EditorConfig } from "ckeditor5";

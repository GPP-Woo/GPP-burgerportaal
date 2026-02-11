import type { App } from "vue";

import {
  PageHeader as UtrechtPageHeader,
  PageFooter as UtrechtPageFooter,
  Link as UtrechtLink,
  SkipLink as UtrechtSkipLink,
  Article as UtrechtArticle,
  Heading as UtrechtHeading,
  Paragraph as UtrechtParagraph,
  UnorderedList as UtrechtUnorderedList,
  UnorderedListItem as UtrechtUnorderedListItem,
  Button as UtrechtButton,
  FormField as UtrechtFormField,
  FormLabel as UtrechtFormLabel,
  Textbox as UtrechtTextbox,
  Textarea as UtrechtTextarea,
  Table as UtrechtTable,
  TableCaption as UtrechtTableCaption,
  TableHeader as UtrechtTableHeader,
  TableBody as UtrechtTableBody,
  TableFooter as UtrechtTableFooter,
  TableRow as UtrechtTableRow,
  TableCell as UtrechtTableCell,
  TableHeaderCell as UtrechtTableHeaderCell,
  Select as UtrechtSelect,
  FormFieldset as UtrechtFieldset,
  FormFieldsetLegend as UtrechtLegend,
  Document as UtrechtDocument,
  DataList as UtrechtDataList,
  DataListItem as UtrechtDataListItem,
  DataListKey as UtrechtDataListKey,
  DataListValue as UtrechtDataListValue
} from "@utrecht/component-library-vue";

const components = {
  UtrechtPageHeader,
  UtrechtPageFooter,
  UtrechtLink,
  UtrechtSkipLink,
  UtrechtArticle,
  UtrechtHeading,
  UtrechtParagraph,
  UtrechtUnorderedList,
  UtrechtUnorderedListItem,
  UtrechtButton,
  UtrechtFormField,
  UtrechtFormLabel,
  UtrechtTextbox,
  UtrechtTextarea,
  UtrechtTable,
  UtrechtTableCaption,
  UtrechtTableHeader,
  UtrechtTableBody,
  UtrechtTableFooter,
  UtrechtTableRow,
  UtrechtTableCell,
  UtrechtTableHeaderCell,
  UtrechtSelect,
  UtrechtFieldset,
  UtrechtLegend,
  UtrechtDocument,
  UtrechtDataList,
  UtrechtDataListItem,
  UtrechtDataListKey,
  UtrechtDataListValue
} as const;

export type OurComponents = typeof components;

export const registerComponents = (app: App): void =>
  Object.entries(components).forEach(([key, value]) => app.component(key, value));

Vue.component('item', {
    template: `
        <div class="row">
            <div class="col-sm-6 row">
                <div class="col-sm-4">
                    <img class="w-100" :src="item.image">
                </div>
                <div class="col-sm-8">
                    <table class="table">
                        <tbody>
                            <tr>
                                <th colspan="2">
                                    <a class="text-reset" :href="originalLabels.url">{{ item.label }}</a>
                                </th>
                            </tr>
                            <tr>
                                <th>
                                    Category:
                                </th>
                                <td>
                                    {{ itemLabels.categoryLabel }} ({{ item.category }})
                                    <br>
                                    <span class="text-secondary">{{ originalLabels.categoryLabel }}</span>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    Brand: 
                                </th>
                                <td>
                                    {{ itemLabels.brandLabel }} ({{ item.brand }})
                                    <br>
                                    <span class="text-secondary">{{ originalLabels.brandLabel }}</span>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    Package:
                                </th>
                                <td>
                                    {{ itemLabels.packageLabel }} ({{ item.package }})
                                    <br>
                                    <span class="text-secondary">{{ originalLabels.packageLabel }}</span>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    Units:
                                </th>
                                <td>
                                    {{ item.units }} {{ itemLabels.packageShort }}
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    Term:
                                </th>
                                <td>
                                    {{ item.term }}
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    Barcodes:
                                </th>
                                <td>
                                    <span v-for="barcode in item.barcodes">{{ barcode }}, </span>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    Consist: 
                                </th>
                                <td>
                                    <span v-for="consistLabel in itemLabels.consistLabels">{{ consistLabel }}, </span> 
                                    (<span v-for="consistId in item.consist">{{ consistId }}, </span>)
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    Calorie:
                                </th>
                                <td>
                                    {{ item.calorie }}
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    Carbohydrates:
                                </th>
                                <td>
                                    {{ item.carbohydrates }}
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    Fat:
                                </th>
                                <td>
                                    {{ item.fat }}
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    Proteins:
                                </th>
                                <td>
                                    {{ item.proteins }}
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    Country:
                                </th>
                                <td>
                                    {{ itemLabels.countryLabel }} ({{ item.additional.country }})
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="col-sm-6 row">
                <div class="col-sm-4" v-if="currentSimilarItem !== null">
                    <img class="w-100" :src="currentSimilarItem.image">
                </div>
                <div class="col-sm-8 mb-3" v-if="currentSimilarItem !== null">
                    <table class="table">
                        <tbody>
                            <tr>
                                <th colspan="2">
                                    {{ currentSimilarItem.label }}
                                </th>
                            </tr>
                            <tr>
                                <th>
                                    Category:
                                </th>
                                <td>
                                    {{ currentSimilarLabels.categoryLabel }} ({{ currentSimilarItem.category }})
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    Brand: 
                                </th>
                                <td>
                                    {{ currentSimilarLabels.brandLabel }} ({{ currentSimilarItem.brand }})
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    Package:
                                </th>
                                <td>
                                    {{ currentSimilarLabels.packageLabel }} ({{ currentSimilarItem.package }})
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    Units:
                                </th>
                                <td>
                                    {{ currentSimilarItem.units }} {{ currentSimilarLabels.packageShort }}
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    Term:
                                </th>
                                <td>
                                    {{ currentSimilarItem.term }}
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    Barcodes:
                                </th>
                                <td>
                                    <span v-for="barcode in currentSimilarItem.barcodes">{{ barcode }}, </span>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    Consist: 
                                </th>
                                <td>
                                    <span v-for="consistLabel in currentSimilarLabels.consistLabels">{{ consistLabel }}, </span> 
                                    (<span v-for="consistId in currentSimilarItem.consist">{{ consistId }}, </span>)
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    Calorie:
                                </th>
                                <td>
                                    {{ currentSimilarItem.calorie }}
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    Carbohydrates:
                                </th>
                                <td>
                                    {{ currentSimilarItem.carbohydrates }}
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    Fat:
                                </th>
                                <td>
                                    {{ currentSimilarItem.fat }}
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    Proteins:
                                </th>
                                <td>
                                    {{ currentSimilarItem.proteins }}
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    Country:
                                </th>
                                <td>
                                    {{ currentSimilarLabels.countryLabel }} ({{ currentSimilarItem.additional.country }})
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div class="input-group">
                    <select class="form-select" v-model="currentSimilarId">
                        <option selected disabled>Chose opinion</option>
                        <option name="item" value="0">New item</option>
                        <option v-for="similarOption in similarOptions" v-bind:value="similarOption.value">{{ similarOption.text }}</option>
                    </select>
                    <button class="btn btn-primary" @click="insert">Insert</button>
                </div>
            </div> 
        </div>
    `,
    data() {
        return {
            currentSimilarId: 0
        }
    },
    methods: {
        insert: function () {
            Vue.prototype.$itemsaver = {
                sourceItem: this.item,
                destinationItem: this.currentSimilarItem,
                saveActive: true,
                originalLabels: this.originalLabels
            }
            this.$emit("itemsaver");
        }
    },
    props: {
        item: {
            type: Object
        },
        similarItems: {
            type: Array
        },
        originalLabels: {
            type: Object
        }
    },
    mounted(){
        this.item.consist = this.item.consist ?? [];
    },
    computed: {
        currentSimilarItem: function () {
            if (this.currentSimilarId == 0) {
                var item = this.item;
                item.consist = item.consist ?? [];
                return item;
            } else {
                var item = this.similarItems[this.currentSimilarId - 1];
                item.consist = item.consist ?? [];
                return item;
            }
        },
        currentSimilarLabels: function () {
            if (this.currentSimilarId == 0) {
                return this.itemLabels;
            } else {
                return this.similarLabels[this.currentSimilarId - 1];
            }
        },
        itemLabels: function () {
            var itemCategoryLabels = this.$labels.categories.filter(value => value.id == this.item.category);
            var itemBrandLabels = this.$labels.brands.filter(value => value.id == this.item.brand);
            var itemPackageLabels = this.$labels.packages.filter(value => value.id == this.item.package);
            var itemConsistLabels = this.item.consist !== null && this.item.consist.length > 0 ? this.$labels.consists.filter(value => this.item.consist.includes(value.id)) : [];
            var itemCountryLabels = this.$labels.countries.filter(value => value.id == this.item.additional.country);
            return {
                categoryLabel: itemCategoryLabels.length > 0 ? itemCategoryLabels[0].label : null,
                brandLabel: itemBrandLabels.length > 0 ? itemBrandLabels[0].label : null,
                packageLabel: itemPackageLabels.length > 0 ? itemPackageLabels[0].label : null,
                packageShort: itemPackageLabels.length > 0 ? itemPackageLabels[0].short : null,
                consistLabels: itemConsistLabels.length > 0 ? itemConsistLabels.map(value => value.label) : [],
                countryLabel: itemCountryLabels.length > 0 ? itemCountryLabels[0].label : null
            };
        },
        similarLabels: function () {
            return this.similarItems.map(similarItem => {
                var categoryLabels = this.$labels.categories.filter(value => value.id == similarItem.category);
                var brandLabels = this.$labels.brands.filter(value => value.id == similarItem.brand);
                var packageLabels = this.$labels.packages.filter(value => value.id == similarItem.package);
                var consistLabels = similarItem.consist !== null && similarItem.consist.length > 0 ? this.$labels.consists.filter(value => similarItem.consist.includes(value.id)) : [];
                var countryLabels = this.$labels.countries.filter(value => value.id == similarItem.additional.country);
                return {
                    categoryLabel: categoryLabels.length > 0 ? categoryLabels[0].label : null,
                    brandLabel: brandLabels.length > 0 ? brandLabels[0].label : null,
                    packageLabel: packageLabels.length > 0 ? packageLabels[0].label : null,
                    packageShort: packageLabels.length > 0 ? packageLabels[0].short : null,
                    consistLabels: consistLabels.length > 0 ? consistLabels.map(value => value.label) : [],
                    countryLabel: countryLabels.length > 0 ? countryLabels[0].label : null
                };
            });
        },
        similarOptions: function () {
            var array = [];
            var i = 1;
            this.similarItems.forEach(element => {
                array.push({ text: element.label, value: i++ });
            });
            return array;
        }
    }
});
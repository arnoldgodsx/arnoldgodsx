[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
Add-Type -Path "CardGenerator.cs" -ReferencedAssemblies "System.Drawing"

[CardGenerator]::GenerateCard(
    "test_card_v2_light.png",
    $false,
    "01",
    "01 / MEMORY",
    "agent-hafiza",
    "Oturumlar arasında bağlamı koruyan, Markdown tabanlı yerel ikinci beyin.",
    "// STACK: LOCAL MARKDOWN ENGINE • CONTEXT GRAPH"
)

[CardGenerator]::GenerateCard(
    "test_card_v2_dark.png",
    $true,
    "01",
    "01 / MEMORY",
    "agent-hafiza",
    "Oturumlar arasında bağlamı koruyan, Markdown tabanlı yerel ikinci beyin.",
    "// STACK: LOCAL MARKDOWN ENGINE • CONTEXT GRAPH"
)

Write-Output "Test card v2 generated successfully"
